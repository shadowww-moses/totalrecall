# DbSet или Context?

При работе с сущностями можно идти двумя путями:

* Работа через DbSet:

  ```c#
  var user = new User(...);
  context.Users.Add(user);
  ```

* Работа сразу через контекст:

  ```c#
  var user = new User(...);
  context.Add(user);
  ```
  
  В случае, когда под сущности нет собственного DbSet'а (например, связующая таблица для образования М:М не нужна в программе и для нее не создавался DbSet), ее можно добавить через контекст.

## Bulk-операции, AddRange

► Второй вариант также имеет преимущество в том, что через контекст можно разом отслеживать сущности разных типов:

```c#
var user = new User(...);
var channel = new Channel(...);
context.AddRange(user, channel);
```

Поскольку типы переменных user и channel известны, EF сам сможет разобраться, как их отслеживать.

DbSet тоже имеет метод `AddRange`, но поскольку DbSet типизирован, можно добавлять только сущности одного типа:

```c#
var Sam = new User(...);
var Tom = new User(...);
var Jack = new User(...);
context.Users.AddRange(Sam, Tom, Jack);
```

► Метод `AddRange` относится к **Bulk-операциям** и позволяет добиться лучшей производительности, чем при множественном вызове методов `Add`. Он также позволяет добавить список объектов:

```c#
var users = new List<User>()
{
    new User() { Username = "Tom" },
    new User() { Username = "Sam" },
    new User() { Username = "Jack" }
};
context.Users.AddRange(users);
context.SaveChanges();
```

Однако производительность повышается только если добавляется 4 и больше элементов, иначе она даже падает.

Методы `Update`, `Remove` и `Attach` тоже имеют Range-версии.
