const user = {name: "Sam"};

function greet(user) {
  const message = "Hello";
  if (user) {
    alert(`${message}, ${user.name}!`);
  }

  function inner() {
    let a = 5;
  }
}

greet(user);
/*
Интересно
var pwd = 'gtgzrflfyct11';
alert(this.pwd);  // gtgzrflfyct11

let pwd = 'gtgzrflfyct11';
alert(this.pwd);  // undefined
*/
