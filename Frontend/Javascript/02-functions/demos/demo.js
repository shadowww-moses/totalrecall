
function sum(x, y) {
  return x + y;
}

console.log(sum(5, 2));

let fact = function f(x) {
  return x > 1
    ? x * f(x-1)
    : 1;
};

console.log(fact(4));

console.log(sum.name);
console.log(fact.name);
