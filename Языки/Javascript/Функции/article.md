# Функции

В материале используется информация о контексте выполнения, лексическом окружении и записи окружения. Детальнее об этом я писал в разделе features/EC.LE.ER

## Предварительная информация

► Функция в Javascript - это *объект специального внутреннего типа FunctionObject*, снабженный семантикой вызова. Поэтому с функциями можно работать как и с обычными данными - передавать в качестве параметров, помещать в переменные.

► Объект функции имеет свойства:

* **name** - хранит имя (идентификатор) функции
* **scope** - хранит ссылку на лексическое окружение выполняемого в данный момент кода

> К слову, имя и идентификатор, по сути, синонимы, поэтому при чтении надо это понимать

► Важно понимать, что единственная строчка кода:

```javascript
let a = 5;
```

Фактически означает две команды:

```javascript
let a;  // Команда объявления переменной
a = 5;  // Команда инициализации переменной значением
```

И самое важное, каждая из этих команд выполняется на принципиально отличных этапах. Первая - на этапе лексического анализа, а вторая - на этапе непосредственного выполнения. Знание этого поможет в понимании отличий между описываемыми далее видами функций.

## Виды функций

Функции условно делятся на:

* Обычные
  * Function Declaration
  * Function Expression
* Стрелки (arrow functions)
* Функции-конструкторы
* new Function

### Обычные

#### Function Declaration

```javascript
function sum(x, y) {  // идентификатор функции - sum
  return x + y;
}  // не нужна точка с запятой
```

#### Function Expression и NFE

```javascript
let sub = function(x, y) {  // здесь не задан идентификатор функции,
  return x - y;             // sub - идентификатор переменной
};  // точка с запятой обязательна
```

В этом примере у функции нет имени, поэтому мы не можем обратиться к ней внутри нее самой. Но можно дать имя, чтобы такая возможность появилась:

```javascript
let fact = function f(x) {  // идентификатор функции - f,
  return x > 1              // а fact - идентификатор переменной
    ? x * f(x-1)
    : 1;
};

console.log(fact(4));  // 24
console.log(f(4));  // ReferenceError: f is not defined
```

Как видно из примера, это имя - `f` - будет доступно только внутри нее самой. Function Expression, которой дано имя, имеет специальное название - **Named Function Expression** или просто **NFE**.

> Есть небольшая тонкость в FE. Вообще-то, даже если имя не задано, то обратиться внутри к самой себе она может, используя переменную, в которую она положена:
>
> ```javascript
> let fact = function(x) {
>   return x > 1
>     ? x * fact(x-1)
>     : 1;
> };
> 
> console.log(fact(4));  // 24
> ```
>
> Однако этот способ ненадежный. Вот как можно сломать такую функцию:
>
> ```javascript
> let factorial = fact;
> fact = null;
> 
> console.log(factorial(4));  // TypeError: fact is not a function
> ```
>
> Функция, будучи помещенной в другую переменную - factorial - при своем выполнении будет все еще искать идентификатор fact, который уже содержит null, а не функцию.
>
> Аналогичная проблема будет для Function Declaration:
>
> ```javascript
> function fact(x) {
>   return x > 1
>     ? x * fact(x-1)
>     : 1;
> };
> 
> let factorial = fact;
> fact = null;
> 
> console.log(factorial(4));  // TypeError: fact is not a function
> ```

#### Разница между FD и FE

Во-первых, для обоих видов функций создается FunctionObject - в этом они абсолютно похожи. Разница в том, *когда* он создается и, соответственно,

* **Момент, в который функция становится доступна для использования**

  Когда какой-нибудь код должен выполниться, перед выполнением движок сперва ищет в нем идентификаторы переменных и функций и помещает идентификаторы этих переменных и функций в объект EnvironmentRecord в качестве полей. Это стадия лексического анализа.

  Когда в коде встречается `function sum(x, y) { }`, *сразу* создается FunctionObject и помещается в ER в поле с именем sum. Поэтому к моменту выполнения кода такая функция уже доступная для использования, даже если объявлена где-нибудь в конце блока.

  Но когда встречается `let sub = function(x, y) { };`, то здесь на самом деле две операции и движок на этапе лексического анализа увидит только объявление переменной sub. Она тоже попадет в ER, но ее значение вычислится потом - только когда код начнет выполняться и дойдет до этой строчки. И вот тогда уже при вычислении значения переменной sub будет создан FunctionObject, он попадет в ER и функция станет доступной для использования. Аналогичная ситуация с `let fact = function f(x) { };` 

* **Область видимости**

  Function Declaration видна только в том блоке, в котором объявлена (и "вглубь", понятное дело, тоже), но не во внешнем. У Ильи Кантора была отмечена эта особенность, но как по мне, это бессмысленно. Ведь FD тоже можно, как и FE, поместить в переменную, объявленную во внешнем блоке, и таким образом сделать видимой.



### Стрелки

```javascript
// Тело в одну строку
let sum = (x, y) => x + y;

// Тело содержит несколько строк
let sub = (x, y) => {
  let result = x - y;
  return result;
};

console.log(sum.name); // sum
```

Если параметр один или их нет вообще:

```javascript
let hello = message => console.log(message);
let goodbye = () => console.log('Goodbye!');

hello('hello, username!');
goodbye();
```

Стрелки удобны, потому что их запись короче, чем Function Expression:

```javascript
let age = 20;

let welcome = (age < 18) ?
  () => alert('Привет') :
  () => alert("Здравствуйте!");

welcome();  // Здравствуйте!
```



### Функции-конструкторы

► Если нужно создать несколько похожих объектов, то вместо того, чтобы каждый раз делать это через литеральную запись, можно воспользоваться синтаксисом *функции-конструктора*:

```javascript
let JohNy = new User("JohNy");  // new, функция с Большой Буквы
let Alice = new User("Alice");
Alice.greet();  // Hello, I'm Alice

function User(name) {
  //this = {};  автоматически неявно
  this.name = name;
  this.isAdmin = false;
    
  this.greet = function() {
    console.log(`Hello, I'm ${name}`);
  }
  //return this;  автоматически неявно
}
```

Для этого перед функцией пишется ключевое слово `new`, а имя функции для удобства принято писать с большой буквы. При таком вызове внутри функции сначала неявно автоматически в this поместится пустой объект, а в конце он будет возвращен.

Как видно из примера, можно помещать в объект как поля, так и методы. Для более продвинутого конструирования объектов существуют *классы*.

► Есть еще один вариант использования функции-конструктора:

```javascript
let JohNy = new function(name) {
  this.name = name;
  this.isAdmin = false;

  message = "current status: ";
  this.status = message + "online";
};

console.log(JohNy.status);
```

В таком случае функцию-конструктор нельзя вызвать несколько раз. Внутри нее можно объявлять локальные переменные, производить любые нужные действия. Это удобно для инкапсуляции логики создания одного сложного объекта, чтобы не захламлять рутиной создания внешнее окружение. Как видно из примера, как-то вызывать эту функцию не нужно - она выполнится при вычислении значения для переменной JohNy и объект JohNy станет сразу доступен для использования.

► Есть возможность проверить, вызвана функция в обычном режиме, или в режиме конструктора:

```javascript
let func = function() {
  if (new.target == undefined) {
    console.log("Функция вызвана обычным способом");
  } else {
    console.log("Функция вызвана в режиме конструктора");
  }
}

func();  // Функция вызвана обычным способом
new func();  // Функция вызвана в режиме конструктора
```

► Обычно в функциях-конструкторах return явно не пишется. Но написать его можно и в случае непонимания как это работает, случайно создать путаницу:

```javascript
let Alice = new User("Alice");

function User(name) {
  this.name = name;
  this.isAdmin = false;
  return { trap: "This is a trap!" };
}

console.log(Alice.name);  // undefined
console.log(Alice.trap);  // This is a trap!
```

Поскольку в return явно указан объект, то значение this проигнорировалось и вернулся именно тот объект, который указан. Поэтому полей name и isAdmin в объекте нет. Но если в return указать примитивное значение или ничего, тогда вернется this.

### new Function

Это специальный синтаксис, который позволяет создать функцию из текстовой строки:

```javascript
let func = new Function([arg1, arg2, ...argN], functionBody);
```

Пример использования:

```javascript
let sum = new Function('a', 'b', 'return a + b');
console.log(sum(1, 2));  // 3

let func = new Function('console.log("Hello, Javascript!")');
func();  // Hello, Javascript!
```

Такие функции имеют ссылку только на глобальное лексическое окружение. [Эта тема у Ильи Кантора](https://learn.javascript.ru/new-function)



## Параметры

### Остаточные параметры

Независимо от того, сколько параметров указано при объявлении функции, передавать в нее можно любое количество аргументов:

```javascript
function sum(x, y, ...rest) {  // оператор ...
  for (let r of rest) {
    console.log(r);
  }
  return x + y;
}

sum(2, 3, 8, 10, 15);
```

Как видно из примера, все "лишние" (правильно говорить *остаточные параметры*) параметры можно  собрать с помощью оператора ... в переменную и затем обойти. Эта конструкция должна быть в конце и после нее не могут идти обычные параметры.

```javascript
function sum(...values) {
  let result = 0;
  for (let v of values) {
    result += v;
  }
  return result;
}

console.log(sum(2, 3, 8, 10, 15));  // 38
```

### arguments

► arguments - это *псевдомассив*, доступный в функции и содержащий все переданные ей аргументы:

```javascript
function demo() {
  for (let arg of arguments) {
    console.log(arg);
  }
}

demo(2, 3, 8, 10, 15, 31);
```

> Псевдомассив - это объект, у которого есть индексы и свойство length, но при этом отсутствуют методы, характерные для массивов (push, pop, shift, unshift, map и т.д.) и, наоборот, могут быть какие-нибудь другие методы, которых у "настоящих" массивов нет

► У функций-стрелок нет своего arguments:

```javascript
let arrow = (x, y, z) => {
  for (let arg in arguments) {
    console.log(arg);
  }
}

arrow(5, 10, 15);  // ReferenceError: arguments is not defined
```

Однако если стрелка вызывается внутри другой функции, она использует ее arguments:

```javascript
function demo() {
  let arrow = (x, y, z) => {
    for (let arg of arguments) {
      console.log(arg);
    }
  }
  arrow();
}

demo(2, 3, 8, 10, 15, 31);  // 2, 3, 8, 10, 15, 31
```

### Оператор расширения (spread, ...)

К любому перебираемому объекту можно применить оператор `...` который разобьет этот объект на отдельные элементы. Применительно к параметрам, это бывает удобно, когда у нас есть набор значений, например, массив, а функция ожидает отдельные значения:

```javascript
let arr1 = [5, 4, 9, 25];
let arr2 = [4, 5, 3];

let max = Math.max(...arr1, ...arr2);
console.log(max);  // 25
```



## Дополнительно

### Свойство length

Для функции означает количество ее параметров:

```javascript
function f1(a) {}
function f2(a, b) {}
function many(a, b, ...more) {}

alert(f1.length); // 1
alert(f2.length); // 2
alert(many.length); // 2
```

Остаточные параметры ... не учитываются в длине.

Пример (потом надо бы придумать более интересный): передаем в функцию вопрос и несколько обработчиков. Обработчики без параметров вызываем только если ответ на вопрос "Да". Обработчики с параметрами вызываем всегда:

```javascript
function ask(question, ...handlers) {
  let isYes = confirm(question);

  for(let handler of handlers) {
    if (handler.length == 0) {
      if (isYes) handler();
    } else {
      handler(isYes);
    }
  }
}

ask("Вопрос?", () => alert('Вы ответили да'), result => alert(result));
```

### Имена функций

```javascript
function sum(x, y) {
  return x + y;
}

let sub = function(x, y) {
  return x - y;
};

let fact = function f(x) {
  return x > 1
    ? x * f(x-1)
    : 1;
};

console.log(sum.name);   // sum
console.log(sub.name);   // sub
console.log(fact.name);  // f
```

Для sum мы указали имя явно, поэтому тут ничего удивительного.

Для sub мы не указали имя явно, но поместили в переменную с именем sub, поэтому имя присвоилось автоматически. Javascript вычислил его, исходя из контекста, и это имя поэтому называется *контекстное имя*.

 Для f мы явно указали имя, поэтому у нее имя f, хотя мы положили ее в переменную fact.

► Обычно функции имеют имена, но бывают случаи, когда вычислить имя не удается и свойство name остается пустым (именно пустым, <empty string>, а не undefined):

```javascript
let arr = [function() {}];
console.log(arr[0].name);
```

### Добавление свойств в функцию

► Поскольку функция - это тоже объект, то мы можем добавить в нее какие-нибудь свойства. Например, количество вызовов:

```javascript
function counter() {
  return ++counter.count;
}

counter.count = 0;
```

Во-первых, свойство добавляется уже после создания объекта функции. Во-вторых, внутри функции обращение к этому свойству идет обязательно с указанием имени функции.

► Свойство - это не локальная переменная. Оно сохраняет свое значение между вызовами:

```javascript
counter();
counter();
counter();
console.log(counter.count);  // 3
```

Таким образом, свойства могут быть использованы в некоторых задачах как альтернатива замыканиям.

Вот как выглядел бы счетчик, реализованный через замыкания:

```javascript
function makeCounter() {
  let count = 0;

  return function() {
    return ++count;
  }
}

let c1 = makeCounter();
let c2 = makeCounter();

console.log(c1());  // 1
console.log(c1());  // 2
console.log(c2());  // 1 независимость счетчиков
```

А вот через свойства:

```javascript
function makeCounter() {
  function counter() {
    return ++counter.count;
  }

  counter.count = 0;

  return counter;
}

let c1 = makeCounter();
let c2 = makeCounter();

console.log(c1.count);  // 0
c1();
c1();
console.log(c1.count);  // 2

console.log(c2.count);  // 0
c2();
console.log(c2.count);  // 1
console.log(c1.count);  // 2 независимость остается

c2.count = 0;  // обнуляем количество вызовов
console.log(c2.count);  // 0
```

Основное отличие в том, что в случае с замыканиями просто взять и посмотреть, сколько раз вызван счетчик - нельзя. Потому что доступ к переменной имеет только сама функция. А в случае со свойством - можно не только посмотреть, но и обнулить вызовы.

> Я пытался получить доступ к количеству вызовов в случае с замыканиями, думал над этим, но то ли чего-то не до конца понимаю в Сэре Эклере, то ли просто из-за оптимизаций движка у меня не работало



## Функции и this

this вообще отдельная задротская супертема. Поэтому здесь пока что напишу только очень вкратце, чтобы можно было пользоваться, а чтобы понимать как это работает - уже потом, иначе закопаюсь.

