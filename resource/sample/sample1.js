
import
   { hello }
   from './module.js';

const DaysEnum = {
    "monday": 1,
    "tuesday": 2,
    "wednesday": 3
};

class Person {

    constructor(firstName, lastName) {
        this._firstName = firstName;
        this._lastName = lastName;
    }

    log(message) {
        let test = "TEST";
        console.log(message, this._firstName, test);
    }

    // setters
    set profession(val) {
        this._profession = val;
    }
    // getters
    get profession() {
        console.log(this._firstName, this._lastName, 'is a', this._profession);
    }
};

// With the above code, even though we can access the properties outside the function to change their content what if we don't want that.
// Symbols come to rescue.
let symbol = new Symbol();

class Math {

    // `PI` is a static public property.
    static PI = 22 / 7; // Close enough.

    // `#totallyRandomNumber` is a static private property.
    static #totallyRandomNumber = 4;

    // `#computeRandomNumber` is a static private method.
    static #computeRandomNumber() {
        return Math.#totallyRandomNumber;
    }

    // `random` is a static public method (ES2015 syntax)
    // that consumes `#computeRandomNumber`.
    static random(message: string, isEnabled) {
        if (isEnabled)
            console.log(message)
        return Math.#computeRandomNumber();
    }
}

class Counter {
    count = 0;
    get value() {
        console.log('Getting the current value!');
        return this.count;
    }

    increment() {
        this.count++;
    }

    decrement() {
        this.count++;
    }
}


await loadJs("https://.../script.js").catch(err => { });

// ES5 syntax
function Person() {
  // we assign `this` to `self` so we can use it later
  var self = this;
  self.age = 0;

  setInterval(function growUp() {
    // `self` refers to the expected object
    self.age++;
  }, 1000);
}

// ES5 syntax
var multiply1 = function(x, y) {
  return x * y;
};

// ES6 arrow function
var multiply2 = (x, y) => { return x * y; };

// Or even simpler
var multiply3 = (x, y) => x * y;   

