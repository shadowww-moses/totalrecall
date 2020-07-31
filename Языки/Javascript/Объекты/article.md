* [Операции с объектами](#operations)
  * [Изменение](#change)



В Javascript, похоже, почти все сделано через объекты, кроме примитивных типов.

Есть два способа создать объект:

```javascript
let user = new Object(); // синтаксис "конструктор объекта"
let user = {};  // синтаксис "литерал объекта"
```

Первый вариант используется редко.



## Свойства

Свойства можно указать прямо при создании объекта:

```javascript
let user = {
  name: "Shadowww-Moses",
  "is new": false,  // если в имени свойства несколько слов, нужны кавычки (любые)
  talisman: { name: "Keytyan" },  // значением может быть объект
  greet: hello,  // или функция, в общем, все что угодно
  age: 31,  // запятую в конце можно ставить по желанию
};

function hello(name) {
  console.log(`Hello, ${name}!`);
}
```

Свойства можно добавлять динамически:

```javascript
user.status = "online";
user["married"] = false;

console.log(user.name);
console.log(user.status);
console.log(user.talisman.name);
console.log(user["is new"]);
console.log(user.married);
user.greet("JohNy");  // Hello, JohNy!
```

И удалять:

```javascript
delete user.status;
delete user["is new"];
console.log(user.status);  // undefined
console.log(user["is new"]);  // undefined
```

Имена свойств должны быть строкой, либо типа Symbol. Если они другого типа, то автоматически преобразуются к строке, как при создании, так и при доступе:

```javascript
let user = {
  0: "JohNy"
};

console.log(user["0"]);  // JohNy
console.log(user[0]);  // JohNy
```



### Вычисляемые свойства

Квадратные скобки дают широкие возможности по формированию свойств:

```javascript
let propName = "fullname";
let part = "name";

let user = {
  [propName]: "JohNy Lain",
  ["user" + part]: "Shadowww-Moses"
};

/*
let user = {
  fullname: "JohNy Lain",
  username: "Shadowww-Moses"
};
*/

console.log(user.fullname);
console.log(user[propName]);
console.log(user.username);
console.log(user["user" + part]);
```

Таким образом, можно динамически формировать имена для свойств при создании объектов и обращаться к свойствам по вычисляемым именам, а не только по заранее известным.

### Есть ли в объекте такое свойство?

Есть два способа проверки, существует ли свойство в объекте:

* Оператор **in**:

  ```javascript
  let user = {
    name: "JohNy"
  };
  
  if ("name" in user) {
    console.log(user.name);
  }
  
  let propName = "surname";
  if (!(propName in user)) {
    console.log(`${propName} property is missing`);
  }
  ```

* Строгое сравнение с undefined:

  ```javascript
  if (user.name === undefined) {
    console.log("'name' property is missing");
  }
  
  let propName = "surname";
  if (user[propName] === undefined) {
    console.log(`${propName} property is missing`);
  }
  ```

  Этот метод сработает для большинства случаев. Но если в объекте все-таки будет свойство surname и его значением будет undefined, тогда мы попадем в ловушку. Впрочем, undefined там может оказаться только если его присвоить явно, что является логической ошибкой, потому что для "пустых" значений нужно использовать null, а не undefined. Поэтому обычно для проверки свойства на существование все-таки используют сравнение с undefined, а in используется реже.

### Перебор свойств

```javascript
let user = {
  name: "JohNy",
  surname: "Lain",
  status: "online"
};

for (let propName in user) {
  console.log(propName + ": " + user[propName]);
}
/*
name: JohNy
surname: Lain
status: online
*/
```

Свойства перебираются в том порядке, в каком добавлены. Единственное исключение - если свойство может быть преобразовано в целое число, то такие свойства будут в начале и по возрастанию. Например:

```javascript
let user = {
  name: "JohNy",
  surname: "Lain",
  "5": "are special",
  status: "online",
  "1": "number properties"
};

for (let propName in user) {
  console.log(propName + ": " + user[propName]);
}
/*
1: number properties
5: are special
name: JohNy
surname: Lain
status: online
*/
```

> Написать демо, которое бы проводило глубокое сравнение объектов, перебирая их свойства



## <a name="operations">Операции с объектами</a>

### Сравнение

Операторы == и === для объектов работают одинаково. Они проводят сравнение по ссылке.

### <a name="change">Изменение</a>

Если объект создать через **const**, это не позволит перезаписать переменную, которая содержит ссылку на него. Но при этом сам объект для редактирования останется доступен:

```javascript
const user = {
  name: "JohNy",
  surname: "Lain"
};

user.name = "Shadowww";  // ok
user.surname = "Moses";  // ok
console.log(`${user.name} ${user.surname}`);  // Shadowww Moses
user = {};  // TypeError: invalid assignment to const 'user'
```

Однако можно сделать и свойства недоступными для изменения

> В процессе. Добавлю, когда изучу главу [Флаги и дескрипторы свойств](https://learn.javascript.ru/property-descriptors).

### Копирование, клонирование

Копирование объектов тоже происходит по ссылке. Поэтому, если нужно создать независимую копию, нужно перебирать свойства и добавлять их в новый объект. 

Для упрощения этого процесса есть метод `Object.assign(dest, src1, src2, ..., srcN)`. Он копирует все свойства из объектов src1-srcN в объект dest и возвращает ссылку на него:

```javascript
let user = {
  name: "JohNy",
  surname: "Lain",
  status: "online"
};

let guest = {
  id: 322,
  status: "offline"
}

// Два синтаксиса
let admin = Object.assign({}, user);  // создаем новый объект admin
let owner = {};
Object.assign(owner, user, guest);  // модифицируем объект owner

/* admin
name: JohNy
surname: Lain
status: online
*/

/* owner
name: JohNy
surname: Lain
status: offline  <-- одинаковые свойства перезаписываются
id: 322
*/
```

Как видно из примера, если в объектах есть свойства с одинаковыми именами, то итоговым значением будет значение из последнего объекта.

`assign` проводит поверхностное копирование. То есть, если значением свойства будет объект, то будет скопирована только ссылка на него. Для глубокого копирования встроенных средств нет, нужно пользоваться библиотеками или писать самому.

> Написать демо с глубоким копированием