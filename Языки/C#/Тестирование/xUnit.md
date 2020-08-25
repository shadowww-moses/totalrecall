# Организация проекта

В примерах используется xUnit. Для работы с ним не нужно ставить дополнительных библиотек, достаточно просто добавить в решение новый проект и в шаблонах выбрать xUnit.

► Тесты пишутся в обычных *public* классах. Тесты - это обычные методы, снабженные атрибутами, например, Fact:

```c#
using Xunit;

namespace Tests.JointGeneratorTests
{
    [Fact]
    public void GetValueTest()
    {
        ...

        Assert.Equal("Мария Склодовская-Кюри", fullName.GetValue());
    }
}
```

► Написанные тесты автоматически становятся видимыми в окне `View > Test Explorer`

► Обычное сравнение ожидаемого\реального значения делается методом `Assert.Equal(ожидание, реальность)`:

```c#
Assert.Equal("Мария Склодовская-Кюри", fullName.GetValue());
```



# Moq

Эту библиотеку можно поставить через NuGet.

[Мануал от Сергея Теплякова](https://habr.com/ru/post/150859/)



► Есть класс, у которого нужно сымитировать работу метода:

```c#
public abstract class Shard
{
    public abstract string GetValue();
}
```

Это делается с помощью `Mock.Of`:

```c#
[Fact]
public void GetValueTest()
{
    Shard nameShard = Mock.Of<Shard>(sh => sh.GetValue() == "Мария");
    Shard surnameShardPt1 = Mock.Of<Shard>(sh => sh.GetValue() == "Склодовская");
    Shard surnameShardPt2 = Mock.Of<Shard>(sh => sh.GetValue() == "Кюри");

    Shard fullName = new ShardComposite(
        nameShard,
        new ShardComposite("-", surnameShardPt1, surnameShardPt2));

    Assert.Equal("Мария Склодовская-Кюри", fullName.GetValue());
}
```

