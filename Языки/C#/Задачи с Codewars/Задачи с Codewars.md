Подборка задач и понравившихся решений



> Напишите функцию, которая принимает строку и возвращает символ, который образует наибольшую непрерывную последовательность, а также длину этой последовательности. Для пустой строки верните null вместо символа и 0 для количества
>
> Например: 
>
> "aaabbcccc" > c, 4
>
> "" > null, 0

```c#
public static (char?, int) LongestRepetition(string input)
{
    return input
        .Select((ch, ind) => 
                new ValueTuple<char?, int>(
                    ch, 
                    input.Substring(ind).TakeWhile(t => t == ch).Count()
                )
        )
        .OrderByDescending(t => t.Item2)
        .FirstOrDefault();
}
```

```c#
public static Tuple<char?, int> LongestRepetition(string input)
{
    return input.Select(
        	(ch, ind) => new Tuple<char?, int>(
	            	ch,
	            	input.Substring(ind).TakeWhile(t => t == ch).Count()
            	)
    	)
        .OrderByDescending(t => t.Item2)
        .FirstOrDefault()
        ?? new Tuple<char?, int>(null, 0);
}
```



> Есть меню "Burger", "Fries", "Chicken", "Pizza". На вход поступает заказ в виде строки "frieschickenfriespizzaburgerpizza". Нужно преобразовать его в красивый вид, чтобы элементы были разделены пробелом, были с большой буквы и шли в том порядке, в котором они идут в меню: "Burger Fries Fries Chicken Pizza Pizza"

```c#
public static string GetOrder(string input)
{
    return string.Join(" ", new string[] { "Burger", "Fries", "Chicken", "Pizza", "Sandwich", "Onionrings", "Milkshake", "Coke" }
                       .SelectMany(food => Enumerable.Repeat(food, Regex.Matches(input, food, RegexOptions.IgnoreCase).Count())));
}
```

```c#
public static string GetOrder(string input)
{
    var menu = new string[] { "Burger", "Fries", "Chicken", "Pizza", "Sandwich", "Onionrings", "Milkshake", "Coke" };
    var order = new List<string>();
    foreach (var item in menu)
    {
        var repeats = Regex.Matches(input, item, RegexOptions.IgnoreCase).Count();
        order.AddRange(Enumerable.Repeat(item, repeats));
    }

    return string.Join(" ", order);
}
```

