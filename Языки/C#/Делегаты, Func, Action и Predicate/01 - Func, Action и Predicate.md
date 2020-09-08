Это все делегаты, объявленные в неймспейсе System примерно вот так:

```c#
public delegate TResult Func<out TResult>();
```

То есть они не самостоятельные классы например, наследующиеся от делегата, а обычные дженерик-делегаты



# Func

Используется для хранения методов, возвращающих значение. Может включать до 16 параметров `Func<T1…T16, TResult>` - возвращаемый тип идет всегда в конце:

```c#
Func<string, int, string> ConcatInfo = (name, age) => $"{name}, {age}";
Func<int> GetFive = () => 5;
```



# Action

Используется для хранения методов, НЕ возвращающих значение. Так же может включать до 16 параметров. Если метод к тому же не принимает ни одного параметра, то используется не дженерик версия делегата:

```c#
Action WriteNothing = () => Console.WriteLine("Nothing");
WriteNothing();

Action<string> PrintSome = 
    (message) => Console.WriteLine(message);
PrintSome("Bye-Bye Borderline");
```



# Predicate

Он всегда возвращает bool. Этот делегат используется в методах некоторых объектов. Например, в методах поиска в списке:

```c#
var arr = new List<int>() { 2, 5, 3, 1, 4 };
arr.FindAll(x => x > 2);
```

А вот как описать его отдельно:

```c#
Predicate<int> odd = (val) => (val % 2) == 0;

var arr = new List<int>() { 2, 5, 3, 1, 4 };
var odds = arr.FindAll(odd).ToList();

odds.ForEach(x => Console.WriteLine(x));
```

