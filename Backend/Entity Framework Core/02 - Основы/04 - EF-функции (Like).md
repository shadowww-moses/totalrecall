# EF-функции

► В EF встроены некоторые функции, делающие запросы более близкими к SQL-стилю.

Их немного, это `Like` и функции для работы с разницами дат.

<img src="img\image-20200507174900592.png" alt="image-20200507174900592" style="zoom:80%;" />

```c#
context.Users.Where(s => EF.Functions.Like(s.Name, "%ova%")).ToList();
```

<img src="img\image-20200507175249150.png" alt="image-20200507175249150" style="zoom:80%;" />

► С помощью EF-функций можно делать и другие вещи. Например, получать значение Shadow-свойств сущностей