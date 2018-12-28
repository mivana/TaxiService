import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../services/userService.service';
import { PasswordValidation } from '../Validators/password-validation.validation';
import { first } from 'rxjs/operators';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
    submitted = false;
    registerForm: FormGroup;
  
    genders: string[] = ['Male','Female'];
    default: string = 'Male';
    showError: boolean = false;
  resultError: boolean = false;
  mailError: boolean;
  
    constructor(private fb: FormBuilder,
                private router: Router,
                private service: UserService) { }
  
    ngOnInit() {
  
      this.registerForm = this.fb.group({
        fullName: ['', Validators.required],
        gender: [null],
        jmbg: [''],
        contactNumber: [''],
        email: ['',[Validators.required,Validators.email]],
        username: ['', Validators.required],
        password: ['',[ 
          // 1. Password Field is Required
           Validators.required,
           // 2. check whether the entered password has a number
           PasswordValidation.patternValidator(/\d/, { hasNumber: true }),
           // 3. check whether the entered password has upper case letter
           PasswordValidation.patternValidator(/[A-Z]/, { hasCapitalCase: true }),
           // 4. check whether the entered password has a lower-case letter
           PasswordValidation.patternValidator(/[a-z]/, { hasSmallCase: true }),
           // 5. check whether the entered password has a special character
           PasswordValidation.patternValidator(/[ !@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/, { hasSpecialCharacters: true }),
           // 6. Has a minimum length of 6 characters
           Validators.minLength(6)
          ]],
        confirmPassword: ['',Validators.required]
      },
      {
        validator: PasswordValidation.MatchPassword
      });
  
      this.registerForm.controls['gender'].setValue(this.default, {onlySelf: true});
    }
    
    
    ngAfterViewInit()
    {
      this.registerForm.valueChanges.subscribe(
        data =>{
          this.submitted = false;
        }
      )
    }

   
    get f() { return this.registerForm.controls; }
  
   
  
    onSubmit() {
      this.submitted = true;
      this.showError = false;
      this.mailError = false;
      this.resultError = false;
  
      // stop here if form is invalid
      if (this.registerForm.invalid) {
          return;
      }
  
      //alert('SUCCESS!! :-)\n\n' + JSON.stringify(this.registerForm.value));
      
            this.service.register(this.registerForm.value,"AppUser")
            .pipe(first())
              .subscribe(
                  data => {
                      this.router.navigate(['userHome']);
                  },
                  error => {
                        var errMesage = error.error;
                        if(errMesage.Message == "Username not unique")
                          this.showError = true;
                        if(errMesage.Message == "Email has account")
                          this.mailError = true;
                        else
                          this.resultError = true;  
                        this.submitted = false;        
                  });
    
    }
  
  
  }
  