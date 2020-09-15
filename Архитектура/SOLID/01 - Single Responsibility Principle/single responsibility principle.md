# Принцип единственной ответственности

SRP заключается в том, что *код должен иметь только одну причину для изменения*. Именно так, только одну причину для изменения, а не "выполнять только одну функцию", потому что это не одно и то же. Код может выполнять несколько функций и при этом не нарушать SRP.

Это возможно в том случае, если класс *делегирует* часть обязанностей другим классам. Тогда при изменении требований правки касаются только того класса, который *непосредственно* решает поставленную задачу, а более высокоуровневый класс остается без изменений.

## Исходный пример

Рассмотрим на примере класса, обрабатывающего акции. Он выполняет разом три функции - читает данные из файла, форматирует их и вставляет в базу данных. Этот класс нарушает SRP не потому, что выполняет сразу три функции, а потому что *содержит реализацию всех трех функций* разом. То есть при изменении деталей любой из функций потребуется править код этого класса:

```c#
public class TradeProcessor
{
    public void ProcessTrades(Stream stream)
    {
        // <-- Читаем данные из файла
        var lines = new List<string>();
        using (var reader = new StreamReader(stream))
        {
            string line;
            while((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
        }

        var trades = new List<TradeRecord>();

        // <-- Анализируем данные, форматируем
        var lineCount = 1;
        foreach (var line in lines)
        {
            var fields = line.Split(new char[] { ',' });

            if(fields.Length != 3)
            {
                Console.WriteLine("WARN: Line {0} malformed. Only {1} field(s) found.", lineCount, fields.Length);
                continue;
            }

            if(fields[0].Length != 6)
            {
                Console.WriteLine("WARN: Trade currencies on line {0} malformed: '{1}'", lineCount, fields[0]);
                continue;
            }

            int tradeAmount;
            if(!int.TryParse(fields[1], out tradeAmount))
            {
                Console.WriteLine("WARN: Trade amount on line {0} not a valid integer: '{1}'", lineCount, fields[1]);
            }

            decimal tradePrice;
            if(!decimal.TryParse(fields[2], out tradePrice))
            {
                Console.WriteLine("WARN: Trade price on line {0} not a valid decimal: '{1}'", lineCount, fields[2]);
            }

            var sourceCurrencyCode = fields[0].Substring(0, 3);
            var destinationCurrencyCode = fields[0].Substring(3, 3);

            var trade = new TradeRecord
            {
                SourceCurrency = sourceCurrencyCode,
                DestinationCurrency = destinationCurrencyCode,
                Lots = tradeAmount / LotSize,
                Price = tradePrice
            };

            trades.Add(trade);

            lineCount++;
        }

        // <-- Записываем информацию в БД
        using (var connection = new System.Data.SqlClient
               .SqlConnection("Data Source=(local);Initial Catalog=TradeDatabase;Integrated Security=True;"))
        {
            connection.Open();
            using(var transaction = connection.BeginTransaction())
            {
                foreach(var trade in trades)
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "dbo.insert_trade";
                    command.Parameters.AddWithValue("@sourceCurrency", trade.SourceCurrency);
                    command.Parameters.AddWithValue("@destinationCurrency", trade.DestinationCurrency);
                    command.Parameters.AddWithValue("@lots", trade.Lots);
                    command.Parameters.AddWithValue("@price", trade.Price);

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            connection.Close();
        }

        Console.WriteLine("INFO: {0} trades processed", trades.Count);
    }

    private static float LotSize = 100000f;
}
```

Исправить ситуацию можно, если вынести код этих трех функций в отдельные классы, а в исходном классе, `TradeProcessor`, оставить только интерфейсы и запросить реализацию извне. В этом случае, если например понадобится считывать данные из другого источника, то TradeProcessor править не придется, потому что теперь он не ответственен за реализацию этого функционала. Общая схема работы "считать-отформатировать-записать" останется неизменной. Таким образом, можно сказать, что TradeProcessor ответственен не за "чтение", "форматирование", "запись", а за *организацию этих трех функций в единый рабочий процесс*. И вот эта организация и становится его единственной ответственностью. Единственной причиной изменения TradeProcessor будет изменение в этом процессе, если к примеру после форматирования нужно будет произвести еще какие-нибудь вычисления.



## Подготовка к рефакторингу

Начать можно с разделения этого монолитного метода на несколько специализированных, оставив все для начала в исходном классе:

```c#
public class TradeProcessor
{
    private IEnumerable<string> ReadTradeData(Stream stream)
    {
        var tradeData = new List<string>();
        using (var reader = new StreamReader(stream))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                tradeData.Add(line);
            }
        }
        return tradeData;
    }

    private IEnumerable<TradeRecord> ParseTrades(IEnumerable<string> tradeData)
    {
        var trades = new List<TradeRecord>();
        var lineCount = 1;
        foreach (var line in tradeData)
        {
            var fields = line.Split(new char[] { ',' });

            if (!ValidateTradeData(fields, lineCount))
            {
                continue;
            }

            var trade = MapTradeDataToTradeRecord(fields);

            trades.Add(trade);

            lineCount++;
        }

        return trades;
    }

    private bool ValidateTradeData(string[] fields, int currentLine)
    {
        if (fields.Length != 3)
        {
            LogMessage("WARN: Line {0} malformed. Only {1} field(s) found.", currentLine, fields.Length);
            return false;
        }

        if (fields[0].Length != 6)
        {
            LogMessage("WARN: Trade currencies on line {0} malformed: '{1}'", currentLine, fields[0]);
            return false;
        }

        int tradeAmount;
        if (!int.TryParse(fields[1], out tradeAmount))
        {
            LogMessage("WARN: Trade amount on line {0} not a valid integer: '{1}'", currentLine, fields[1]);
            return false;
        }

        decimal tradePrice;
        if (!decimal.TryParse(fields[2], out tradePrice))
        {
            LogMessage("WARN: Trade price on line {0} not a valid decimal: '{1}'", currentLine, fields[2]);
            return false;
        }

        return true;
    }

    private void LogMessage(string message, params object[] args)
    {
        Console.WriteLine(message, args);
    }

    private TradeRecord MapTradeDataToTradeRecord(string[] fields)
    {
        var sourceCurrencyCode = fields[0].Substring(0, 3);
        var destinationCurrencyCode = fields[0].Substring(3, 3);
        var tradeAmount = int.Parse(fields[1]);
        var tradePrice = decimal.Parse(fields[2]);

        var trade = new TradeRecord
        {
            SourceCurrency = sourceCurrencyCode,
            DestinationCurrency = destinationCurrencyCode,
            Lots = tradeAmount / LotSize,
            Price = tradePrice
        };

        return trade;
    }

    private void StoreTrades(IEnumerable<TradeRecord> trades)
    {
        using (var connection = new System.Data.SqlClient
               .SqlConnection("Data Source=(local);Initial Catalog=TradeDatabase;Integrated Security=True;"))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                foreach (var trade in trades)
                {
                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "dbo.insert_trade";
                    command.Parameters.AddWithValue("@sourceCurrency", trade.SourceCurrency);
                    command.Parameters.AddWithValue("@destinationCurrency", trade.DestinationCurrency);
                    command.Parameters.AddWithValue("@lots", trade.Lots);
                    command.Parameters.AddWithValue("@price", trade.Price);

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            connection.Close();
        }

        LogMessage("INFO: {0} trades processed", trades.Count());
    }

    public void ProcessTrades(Stream stream)
    {
        var lines = ReadTradeData(stream);
        var trades = ParseTrades(lines);
        StoreTrades(trades);
    }

    private static float LotSize = 100000f;
}
```



## Рефакторинг к абстракциям

