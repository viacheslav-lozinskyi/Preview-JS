declare namespace D3 {
    export interface Event {
        x: number;
        y: number;
    }

    export interface Selectors {
        select: {
            (selector: string): Selection;
            (element: EventTarget): Selection;
        };
    }

    export interface Base extends Selectors {
        event: Event;
    }
}

export interface StringValidator {
    isAcceptable(s: string): boolean;
}

export const numberRegexp = /^[0-9]+$/;
 
export default class ZipCodeValidator {
    static numberRegexp = /^[0-9]+$/;

    private privateMethod() { }

    protected isAcceptable(message: string) {
        return message.length === 5 && ZipCodeValidator.numberRegexp.test(message);
    }
}

export { ZipCodeValidator };
export { ZipCodeValidator as mainValidator };

import { ZipCodeValidator as ZCV } from "./ZipCodeValidator";
import * as validator from "./ZipCodeValidator";

let myValidator = new validator.ZipCodeValidator();

import "./my-module.js";
import $ from "JQuery";

import validator from "./ZipCodeValidator";
 
let myValidator = new validator();

