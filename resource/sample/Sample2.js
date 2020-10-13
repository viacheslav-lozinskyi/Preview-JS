// outer() is in scope here because functions can be forward-referenced

function outer() {

    // only inner() is in scope here
    // because only functions are forward-referenced

    var a = 1;

    //now 'a' and inner() are in scope

    function inner() {
        var b = 2

        if (a == 1) {
            var c = 3;
        }

        // 'c' is still in scope because JavaScript doesn't care
        // about the end of the 'if' block, only function inner()
    }

    // now b and c are out of scope
    // a and inner() are still in scope

}

function createQuote(quote, callback) { 
  var myQuote = "Like I always say, " + quote;
  callback(myQuote); // 2
}

function createQuote(quote, functionToCall) { 
  var myQuote = "Like I always say, " + quote;
  functionToCall(myQuote);
}

function logQuote(quote){
  console.log(quote);
}

createQuote("eat your vegetables!", logQuote); // 1

createQuote("eat your vegetables!", function(quote) {
  console.log(quote); 
});


// Result in console: 
// Like I always say, eat your vegetables!

// ES6 syntax
function Person(){
  this.age = 0;

  setInterval(() => {
    // `this` now refers to the Person object, brilliant!
    this.age++;
  }, 1000);
}

var person = new Person();
