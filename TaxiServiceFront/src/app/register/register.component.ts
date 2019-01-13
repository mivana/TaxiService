import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../services/userService.service';
import { PasswordValidation } from '../Validators/password-validation.validation';
import { first } from 'rxjs/operators';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { Router } from '@angular/router';
import { SessionService } from '../services/sessionservice.service';
import { AuthenticationService } from '../services/authenticationservice.service';

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
  showResult: boolean = false;
  showErrorL: boolean = false;
  
    constructor(private fb: FormBuilder,
                private router: Router,
                private service: UserService,
                private authService: AuthenticationService) { }
  
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
  
      this.service.register(this.registerForm.value,"AppUser")
      .pipe(first())
      .subscribe(
        data => {
          this.authService.login(this.f.email.value,this.f.password.value).subscribe(
            res => {
                console.log(res.access_token);
      
                let jwt = res.access_token;
      
                let jwtData = jwt.split('.')[1]
                let decodedJwtJsonData = window.atob(jwtData)
                let decodedJwtData = JSON.parse(decodedJwtJsonData)
      
                let role = decodedJwtData.role
      
                console.log('jwtData: ' + jwtData)
                console.log('decodedJwtJsonData: ' + decodedJwtJsonData)
                console.log('decodedJwtData: ' + decodedJwtData)
                console.log('Role ' + role)
      
                localStorage.setItem('jwt', jwt)
                localStorage.setItem('role', role);
      
                    this.router.navigate(['/userHome']);
      
            },
            error => {
                  console.log("Error occured");
                  if(error.error.error_description == "The user name or password is incorrect.!!!!")
                      this.showResult = true;
                  else   
                      this.showErrorL = true;
                  this.submitted = false;
              });
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
