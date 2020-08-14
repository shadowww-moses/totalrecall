Наследоваться можно только от одного класса:

```c#
class Person {
}

class User : Person {
}
```



# Наследование и private члены

Номинально считается, что наследуются только public и protected члены.

Но приватные тоже "наследуются", однако недоступны потомку для использования напрямую. Факт, что они все-таки есть у объекта потомка и с ними можно работать, можно увидеть следующим образом:

```c#
class Person
{
    public string Name { get; private set; } = "Tony";
    protected int Age { get; private set; } = 40;
    private string City { get; set; } = "Liberty City";

    public Person() { }
    
    public Person(string name, int age, string city)
    {
        Name = name;
        Age = age;
        City = city;
    }

    public void ChangeCity(string newCity)
    {
        City = newCity;
    }

    public string ExtractCity()
    {
        return City;
    }
}

class User : Person
{

}

var user = new User();
Console.WriteLine(user.City);  // 'Person.City' is inaccessible due to its protection level
Console.WriteLine(user.ExtractCity());  // Liberty City, т.е. поле City у объекта user все же есть, иначе ему неоткуда взяться, ведь мы не создавали объект Person
user.ChangeCity("San Andreas");
Console.WriteLine(user.ExtractCity());  // San Andreas
```



# virtual, override и new

Потомок может "переиначивать" метод родителя, предоставляя собственную реализацию:

```c#
class Person
{
    public Person() { }

    public void GetInfo()
    {
        Console.WriteLine("Person GetInfo");
    }

    public virtual void GetFullInfo()  // Планируем переопределять
    {
        Console.WriteLine("Person GetFullInfo");
    }
}

class User : Person
{
    public new void GetInfo()  // Сокрытие
    {
        Console.WriteLine("User GetInfo");
    }

    public override void GetFullInfo()  // Переопределение
    {
        Console.WriteLine("User GetFullInfo");
    }
}
```

Здесь показаны две ситуации: в обоих случаях и родитель, и потомок имеют два метода с одинаковой сигнатурой. Поведение при *сокрытии* и *переопределении* отличается, разберем на примерах:

```c#
User userAsUser = new User();
Person userAsPerson = new User();

// Hiding
userAsUser.GetInfo();  // User GetInfo
userAsPerson.GetInfo();  // Person GetInfo

// Overriding
userAsUser.GetFullInfo();  // User GetFullInfo
userAsPerson.GetFullInfo();  // User GetFullInfo
```

Здесь создаются два объекта типа User, но ссылки на них помещаются в переменные разных типов. И по результатам видно, что в случае сокрытия вызывается метод в зависимости от типа переменной - если она User, то вызывается метод класса User, если ссылка Person, то метод класса Person. А в случае переопределения вызывается метод в зависимости от типа объекта - в обоих случаях срабатывает метод класса User, потому что сам объект имеет именно этот тип, хоть даже и помещен в переменную типа Person. 



# Ссылка base

Потомок может обращаться к родительским версиям унаследованных членов через ссылку `base`:

```c#
class Person
{
    protected string name = "Tony";
    private int age = 40;
    
    public Person() { }

    public void GetInfo()
    {
        Console.WriteLine("Person GetInfo");
    }

    public virtual void GetFullInfo()
    {
        Console.WriteLine("Person GetFullInfo");
    }
}

class User : Person
{
    public new void GetInfo()
    {
        Console.WriteLine("User GetInfo");
    }

    public override void GetFullInfo()
    {
        base.GetFullInfo();  // <-- Person GetFullInfo
        Console.WriteLine(base.name);  // <-- Tony
        Console.WriteLine(base.age);  // <-- Ошибка, age private
        Console.WriteLine("User GetFullInfo");
    }
}
```

Например, если потомок реализует виртуальный метод не абсолютно иначе, а только немного расширяет его, то может быть полезным вызвать родительскую реализацию и дополнить ее, вместо того, чтобы дублировать код. Через base доступны все родительские реализации унаследованных членов - не только методы, но и поля, свойства.