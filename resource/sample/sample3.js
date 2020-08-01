
function Person(firstName, lastName) {
    this._firstName = firstName;
    this._lastName = lastName;
}

Person.prototype.log = function() {
    console.log('I am', this._firstName, this._lastName);
}

// This line adds getters and setters for the profession object. Note that in general you could just write your own get and set functions like the 'log' method above.
// Since in this example we are trying the mimic the class above, we try to use the getters and setters property provided by JavaScript
Object.defineProperty(Person.prototype, 'profession', {
    set: function(val) {
        this._profession = val;
    },
    get: function() {
        console.log(this._firstName, this._lastName, 'is a', this._profession);
    }
})

Person.prototype = {
    log: function() {
        console.log('I am ', this._firstName, this._lastName);
    }
    set profession(val) {
        this._profession = val;
    }

    get profession() {
        console.log(this._firstName, this._lastName, 'is a', this._profession);
    }

}

