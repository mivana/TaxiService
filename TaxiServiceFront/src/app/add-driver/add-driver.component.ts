import { Component, OnInit } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../services/userService.service';
import { PasswordValidation } from '../Validators/password-validation.validation';
import { AdminAuthGuard } from '../guards/admin.guard';
import { Car } from '../models/Car.model';

@Component({
  selector: 'app-add-driver',
  templateUrl: './add-driver.component.html',
  styleUrls: ['./add-driver.component.css'],
  providers:[
    AdminAuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
  ]
})
export class AddDriverComponent implements OnInit {
  submitted = false;
  driverForm: FormGroup;

  genders: string[] = ['Male','Female'];
  default: string = 'Male';

  carTypes: string[] = ['Standard','Combi'];
  defaultC: string = 'Standard';

  showError: boolean = false;
  resultError: boolean = false;
  mailError: boolean;
  result: any;

  constructor(private fb: FormBuilder,
              private router: Router,
              private service: UserService) { }

  ngOnInit() {
    this.driverForm = this.fb.group({
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
      confirmPassword: ['',Validators.required],
      registrationPlate: ['', Validators.required],
      taxiNumber: ['', Validators.required],
      yearMade: ['', Validators.required],
      carType:[null]
    },
    {
      validator: PasswordValidation.MatchPassword
    });

    this.driverForm.controls['gender'].setValue(this.default, {onlySelf: true});
    this.driverForm.controls['carType'].setValue(this.defaultC, {onlySelf: true});
  }

  ngAfterViewInit()
  {
    this.driverForm.valueChanges.subscribe(
      data =>{
        this.submitted = false;
        this.result = false;
      }
    )
  }

  get f() { return this.driverForm.controls; }

  onSubmit() {
    this.submitted = true;
    this.showError = false;
    this.mailError = false;
    this.resultError = false;

    // stop here if form is invalid
    if (this.driverForm.invalid) {
        return;
    }
    
    this.service.registerDriver(this.driverForm.value, "Driver").subscribe(
      data => {
        alert("YES");
      }
    )


          // this.service.register(this.driverForm.value,"Driver")
          // .pipe(first())
          //   .subscribe(
          //       data => {
          //         this.result = true;
          //       },
          //       error => {
          //             var errMesage = error.error;
          //             if(errMesage.Message == "Username not unique")
          //               this.showError = true;
          //             if(errMesage.Message == "Email has account")
          //               this.mailError = true;
          //             else
          //               this.resultError = true;  
          //             this.submitted = false;        
          //       });
  
  }



}
