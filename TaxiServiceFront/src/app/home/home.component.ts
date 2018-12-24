import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { AuthenticationService } from '../services/authenticationservice.service';
import { PasswordValidation } from '../Validators/password-validation.validation';
import { UserService } from '../services/userService.service';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  submitted = false;
  registerForm: FormGroup;

  genders: string[] = ['Male','Female'];
  default: string = 'Male';
  showError: boolean;

  constructor(private fb: FormBuilder,
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
  
 
  get f() { return this.registerForm.controls; }

 

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.registerForm.invalid) {
        return;
    }

    //alert('SUCCESS!! :-)\n\n' + JSON.stringify(this.registerForm.value));
    
          this.service.register(this.registerForm.value,"AppUser")
          .pipe(first())
            .subscribe(
                data => {
                      alert('SUCCESS!! :-)\n\n' + JSON.stringify(this.registerForm.value));
                },
                error => {
                      var errMesage = error.error;
                      
                });
  
  }


}
