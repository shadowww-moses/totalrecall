# Приведение типов

Применительно к классам, приведение типов бывает двух видов:

► **Вверх**, когда ссылка на потомка приводится к ссылке родителя. Такое приведение, во-первых, выполняется *неявно* (автоматически), а, во-вторых, оно всегда успешно:

```c#
class Person { }
class User : Person { }
class Admin : Person { }

User user = new User();  // Переменная user - это ссылка на объект потомка
Person person = user;  // Приводим ее "вверх" - к типу родителя
```

Обычный случай, когда объект потомка кладется в переменную родительского типа. На этом держится полиморфизм.

► **Вниз**, когда ссылка родителя приводится к ссылке потомка. Такое приведение осуществляется вручную и завершается успешно только если приведение осуществляется к правильному типу:

```c#
User user1 = new User();
Person person = user1;  // Приведение вверх

User user2 = person;  // Ошибка компиляции:
// Cannot implicitly convert type 'Person' to 'User'. 
// An explicit conversion exists (are you missing a cast?)
User user2 = (User)person;  // Приведение вниз, указали целевой тип явно, теперь Ok

Admin admin = (User)person;  // Ошибка рантайма:
// System.InvalidCastException: 'Unable to cast object of type 'User' to type 'Admin'
// В переменной person лежит объект типа User, поэтому его нельзя преобразовать к Admin
```



# Ссылки и реальные объекты

Важно понимать различие между типом ссылки и типом того объекта, на который она указывает:

```c#
var person = new Person();
Console.WriteLine(person.GetType());  // Person
var user = new User();
Console.WriteLine(user.GetType());  // User

person = user;
Console.WriteLine(person.GetType());  // User
user = (User)person;
Console.WriteLine(user.GetType());  // User
```

То есть при приведении изменяется именно *тип ссылки*, а не *тип объекта*. Как видно, объект user успешно переживает все манипуляции с типом ссылок и остается юзером.

Вот еще пример, демонстрирующий, что при приведении ссылок сами объекты не "урезаются" и тип свой не меняют:

```c#
class Person
{
    public void WhoAmI()
    {
        Console.WriteLine("I'm a Person");
    }
}

class User : Person 
{ 
    public void SayHello()
    {
        Console.WriteLine("User says 'Hello' to you");
    }
}

User user = new User();
user.SayHello();  // User says 'Hello' to you

Person person = user;
person.SayHello();  // Ошибка компиляции:
// 'Person' does not contain a definition for 'SayHello'
User user2 = (User)person;  // Восстанавливаем ссылку
user2.SayHello();  // User says 'Hello' to you
```

Просто некоторый функционал объектов может быть не виден через родительскую ссылку, как в случае с методом `SayHello`. Поэтому и может потребоваться обратное приведение родительской ссылки к дочерней, чтобы этот функционал снова стал доступен.



# is

Позволяет удобно проверить тип объекта:

```c#
Person person = new User();
if (person is User) 
{
    user = (User)person;
    Console.WriteLine("Преобразование успешно");
}
```

Ошибок не выдает. Если ссылка null или объект другого типа, выдает false.

Есть удобная фича:

```c#
Person person = new User();
if (person is User us)  // <-- us
{
	us.SayHello();
    Console.WriteLine("Преобразование успешно");
}
```

Если тип тот, что нужен, то можно сразу провести преобразование и положить значение в переменную.



# as

Явная конвертация, например `var user = (User)person` дает ошибку рантайма при невозможности преобразования. Оператор `as`  в этом случае просто выдает null:

```c#
Person person = new Admin();
var user = person as User;  // null
if (user != null)
{
    user.SayHello();  // admin не user, здороваться не умеет
}
```



#  typeof

Тип объектов представлен специальным типом `System.Type`. С помощью оператора `typeof` можно получить объект этого типа для пользовательского типа. Звучит запутанно, но иначе и не скажешь, а на деле все просто:

```c#
var user = new User();
var person = new Person();

var userType = typeof(User);
var personType = typeof(Person);

Console.WriteLine(userType.GetType());  // System.RuntimeType
Console.WriteLine(userType.Name);  // User

Console.WriteLine(personType.GetType());  // System.RuntimeType
Console.WriteLine(personType.Name);  // Person
```

То есть typeof принимает тип (не переменную, а именно сам тип) и возвращает для него объект System.Type. А уже из этого объекта можно выудить название типа в виде строки, его пространство имен и прочую информацию.