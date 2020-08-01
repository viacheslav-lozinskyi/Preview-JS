namespace Validation {
    export interface StringValidator {
        isAcceptable(s: string): boolean;
    }

    const lettersRegexp = /^[A-Za-z]+$/;
    const numberRegexp = /^[0-9]+$/;

    export class LettersOnlyValidator implements StringValidator {
        isAcceptable(s: string) {
            return lettersRegexp.test(s);
        }
    }

    export class ZipCodeValidator implements StringValidator {
        isAcceptable(s: string) {
            return s.length === 5 && numberRegexp.test(s);
        }
    }
}

export namespace Shapes {
    export class Triangle { /* ... */ }
    export class Square { /* ... */ }
}

import * as shapes from "./shapes";

enum Direction {
    Up = 1,
    Down,
    Left,
    Right
}

declare enum FileAccess {
    // константные элементы
    None,
    Read    = 1 << 1,
    Write   = 1 << 2,
    ReadWrite  = Read | Write,
    // вычисляемые элементы
    Green = "123".length
}

export class Rectangle { /* ... */ }
export class Polygon { /* ... */ }

function printLabel(labelledObj: { label: string }) {
    console.log(labelledObj.label);
}
 
let myObj = {size: 10, label: "Size 10 Object"};
printLabel(myObj);

interface SquareConfig {
    color?: string;
    width?: number;
}
 
function createSquare(config: SquareConfig): {color: string; area: number} {
    let newSquare = {color: "white", area: 100};
    if (config.color) {
        newSquare.color = config.color;
    }
    if (config.width) {
        newSquare.area = config.width * config.width;
    }
    return newSquare;
}

function loggingIdentity<t>(arg: T): T {
    console.log(arg.length);  // Ошибка: у T нет свойства .length
    return arg;
}
 
// Несколько тестовых примеров
let strings = ["Hello", "98052", "101"];
 
// Валидаторы
let validators: { [s: string]: Validation.StringValidator; } = {};
