Наследоваться можно только от одного класса. 

# Наследование и private

Приватные члены тоже "наследуются", но напрямую недоступны для использования. Однако то, что они все-таки есть у объекта потомка и с ними можно работать, можно увидеть следующим образом:

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
user.ChangeCity("San Andreas");
Console.WriteLine(user.ExtractCity());  // San Andreas
```

