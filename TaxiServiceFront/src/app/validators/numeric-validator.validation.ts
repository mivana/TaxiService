import { AbstractControl } from '@angular/forms';

export class NumbericValidation{
     // Number only validation   
    // static NumericNumber(control: AbstractControl) {
    //     let num = control.get('number').value;
    //     if(Number.isInteger(num))
    //         return null; //if is Number  

    //     //If it is NOT a number
    //     if(!Number.isInteger(num))
    //         control.get('number').setErrors({IsNumber: true})

    // } 

    // static NumericAreaCode(control: AbstractControl){
    //     let areaCode = control.get('areaCode').value
    //     if(Number.isInteger(areaCode))
    //         return null; //if is Number  

    //     //If it is NOT a number
    //     if(!Number.isInteger(areaCode))
    //     control.get('areaCode').setErrors({IsNumber: true})
    // }

    // Validates numbers
    static numberValidator(number): any {
    if (number.pristine) {
       return null;
    }
    const NUMBER_REGEXP = /^-?[\d.]+(?:e-?\d+)?$/;
    number.markAsTouched();
    if (NUMBER_REGEXP.test(number.value)) {
       return null;
    }
    return {
       invalidNumber: true
    };
 }


  }