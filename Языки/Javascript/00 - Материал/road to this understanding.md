# Разница объявлений переменных в строгом и обычном режимах

```javascript
a = 5;
console.log(this.a);  // 5
```

```javascript
'use strict';
a = 5;
console.log(this.a); //ReferenceError: assignment to undeclared variable a
```



