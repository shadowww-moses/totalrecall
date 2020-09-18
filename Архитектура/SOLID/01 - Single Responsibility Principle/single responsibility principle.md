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

Исправить ситуацию можно, если вынести код этих трех функций в отдельные классы, а в исходном классе, `TradeProcessor`, оставить только интерфейсы и запросить реализацию извне. В этом случае, если например понадобится считывать данные из другого источника - не из файла, а из БД - то TradeProcessor править не придется, потому что он не будет ответственен за реализацию этого функционала. Общая схема работы "считать-отформатировать-записать" останется неизменной. 

Таким образом, можно сказать, что TradeProcessor ответственен не за "чтение", "форматирование", "запись", а за *организацию этих трех функций в единый рабочий процесс*. И вот эта организация и становится его единственной ответственностью. Единственной причиной изменения TradeProcessor будет изменение в этом процессе, если к примеру после форматирования нужно будет произвести еще какие-нибудь вычисления.



## Подготовка к рефакторингу

Начать можно с разделения этого монолитного метода на несколько специализированных, оставив все для начала в исходном классе:

```c#
public class TradeProcessor
{
    // Этот метод будет выделен в абстракцию ITradeDataProvider
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

    // Этот метод - в абстракцию ITradeParser
    private IEnumerable<TradeRecord> ParseTrades(IEnumerable<string> tradeData)
    {
        var trades = new List<TradeRecord>();
        var lineCount = 1;
        foreach (var line in tradeData)
        {
            var fields = line.Split(new char[] { ',' });

            // Валидацию тоже делегируем, максимально дробя функционал на самостоятельные части
            if (!ValidateTradeData(fields, lineCount))
            {
                continue;
            }

            // И форматирование тоже
            var trade = MapTradeDataToTradeRecord(fields);

            trades.Add(trade);

            lineCount++;
        }

        return trades;
    }

    // Будущая абстракция ITradeValidator
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

    // ILogger позволит в высокоуровневом классе отвязаться от консоли
    private void LogMessage(string message, params object[] args)
    {
        Console.WriteLine(message, args);
    }

    // Сопоставлением полей займется абстракция ITradeMapper
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

    // Персистентность нам обеспечит абстракция ITradeStorage
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

    // В итоге весь процесс обработки сведется к вызову всего трех высокоуровневых команд,
    // которые и останутся в классе TradeProcessor
    public void ProcessTrades(Stream stream)
    {
        var lines = ReadTradeData(stream);
        var trades = ParseTrades(lines);
        StoreTrades(trades);
    }

    private static float LotSize = 100000f;
}
```

```c#
// Тип для отдельной биржевой записи
public class TradeRecord
{
    public string DestinationCurrency;
    public float Lots;
    public decimal Price;
    public string SourceCurrency;
}
```



## Рефакторинг к абстракциям

Обозначим абстракции с помощью интерфейсов и напишем их реализации

### ITradeDataProvider

```c#
public interface ITradeDataProvider
{
    IEnumerable<string> GetTradeData();
}
```

```c#
public class StreamTradeDataProvider : ITradeDataProvider
{
    private readonly Stream stream;
    
    public StreamTradeDataProvider(Stream stream)
    {
        this.stream = stream;
    }

    public IEnumerable<string> GetTradeData()
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
}
```



### ITradeParser

```c#
public interface ITradeParser
{
    IEnumerable<TradeRecord> Parse(IEnumerable<string> tradeData);
}
```

```c#
public class SimpleTradeParser : ITradeParser
{
    private readonly ITradeValidator tradeValidator;
    private readonly ITradeMapper tradeMapper;

    public SimpleTradeParser(ITradeValidator tradeValidator, ITradeMapper tradeMapper)
    {
        this.tradeValidator = tradeValidator;
        this.tradeMapper = tradeMapper;
    }

    public IEnumerable<TradeRecord> Parse(IEnumerable<string> tradeData)
    {
        var trades = new List<TradeRecord>();
        var lineCount = 1;
        foreach (var line in tradeData)
        {
            var fields = line.Split(new char[] { ',' });

            if (!tradeValidator.Validate(fields))
            {
                continue;
            }

            var trade = tradeMapper.Map(fields);

            trades.Add(trade);

            lineCount++;
        }

        return trades;
    }
}
```



### ITradeValidator

```c#
public interface ITradeValidator
{
    bool Validate(string[] tradeData);
}
```

```c#
public class SimpleTradeValidator : ITradeValidator
{
    private readonly ILogger logger;

    public SimpleTradeValidator(ILogger logger)
    {
        this.logger = logger;
    }

    public bool Validate(string[] tradeData)
    {
        if (tradeData.Length != 3)
        {
            logger.LogWarning("Line malformed. Only {0} field(s) found.", tradeData.Length);
            return false;
        }

        if (tradeData[0].Length != 6)
        {
            logger.LogWarning("Trade currencies malformed: '{0}'", tradeData[0]);
            return false;
        }

        int tradeAmount;
        if (!int.TryParse(tradeData[1], out tradeAmount))
        {
            logger.LogWarning("Trade not a valid integer: '{0}'", tradeData[1]);
            return false;
        }

        decimal tradePrice;
        if (!decimal.TryParse(tradeData[2], out tradePrice))
        {
            logger.LogWarning("Trade price not a valid decimal: '{0}'", tradeData[2]);
            return false;
        }

        return true;
    }
}
```



### ITradeMapper

```c#
public interface ITradeMapper
{
    TradeRecord Map(string[] fields);
}
```

```c#
public class SimpleTradeMapper : ITradeMapper
{
    private static float LotSize = 100000f;
    
    public TradeRecord Map(string[] fields)
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
}
```



### ILogger

```c#
public interface ILogger
{
    void LogWarning(string message, params object[] args);

    void LogInfo(string message, params object[] args);
}
```

```c#
public class ConsoleLogger : ILogger
{
    public void LogWarning(string message, params object[] args)
    {
        Console.WriteLine(string.Concat("WARN: ", message), args);
    }

    public void LogInfo(string message, params object[] args)
    {
        Console.WriteLine(string.Concat("INFO: ", message), args);
    }
}
```



### ITradeStorage

```c#
public interface ITradeStorage
{
    void Persist(IEnumerable<TradeRecord> trades);
}
```

```c#
public class AdoNetTradeStorage : ITradeStorage
{
    private readonly ILogger logger;

    public AdoNetTradeStorage(ILogger logger)
    {
        this.logger = logger;
    }

    public void Persist(IEnumerable<TradeRecord> trades)
    {
        using (var connection = new System.Data
               .SqlClient.SqlConnection("Data Source=(local);Initial Catalog=TradeDatabase;Integrated Security=True;"))
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

        logger.LogInfo("{0} trades processed", trades.Count());
    }
}
```



## Собираем их всех вместе

Вот таким компактным становится класс TradeProcessor после делегирования обязанностей специализированным классам:

```c#
public class TradeProcessor
{
    private readonly ITradeDataProvider tradeDataProvider;
    private readonly ITradeParser tradeParser;
    private readonly ITradeStorage tradeStorage;
    
    public TradeProcessor(ITradeDataProvider tradeDataProvider, 
                          ITradeParser tradeParser, 
                          ITradeStorage tradeStorage)
    {
        this.tradeDataProvider = tradeDataProvider;
        this.tradeParser = tradeParser;
        this.tradeStorage = tradeStorage;
    }

    public void ProcessTrades()
    {
        var lines = tradeDataProvider.GetTradeData();
        var trades = tradeParser.Parse(lines);
        tradeStorage.Persist(trades);
    }
}
```

Теперь остается создать экземпляры всех абстракций и правильно соединить из друг с другом:

```c#
class Program
{
    static void Main(string[] args)
    {
        var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SingleResponsibilityPrinciple.trades.txt");

        var logger = new ConsoleLogger();
        var tradeValidator = new SimpleTradeValidator(logger);
        var tradeDataProvider = new StreamTradeDataProvider(tradeStream);
        var tradeMapper = new SimpleTradeMapper();
        var tradeParser = new SimpleTradeParser(tradeValidator, tradeMapper);
        var tradeStorage = new AdoNetTradeStorage(logger);

        var tradeProcessor = new TradeProcessor(tradeDataProvider, tradeParser, tradeStorage);
        tradeProcessor.ProcessTrades();

        Console.ReadKey();
    }
}
```



## Вывод

В итоге класс TradeProcessor все еще занимается тремя вещами - получает данные из источника, форматирует их и записывает в хранилище. Но теперь реализация этих вещей скрыта за абстракциями и в случае изменений требований к любому компоненту правки затронут только этот компонент, но не TradeProcessor, в отличие от исходного примера. Поэтому TradeProcessor теперь соответствует SRP в полной мере.