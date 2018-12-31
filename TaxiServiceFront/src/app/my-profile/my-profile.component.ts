import { Component, OnInit } from '@angular/core';
import { User } from '../models/User.model';
import { UserService } from '../services/userService.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PasswordValidation } from '../Validators/password-validation.validation';
import { error } from 'util';
import { AllUsersGuard } from '../guards/allUsers.guard';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';

@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.css'],
  providers:[
    AllUsersGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
  ]
})
export class MyProfileComponent implements OnInit {
  activeUser: User;
  editedUser: User;
  tempUser: User;
  showEdit: boolean = false;
  genders: string[] = ['Male','Female'];
  default: string = 'Male';
  oldUsername: string;

  submitted: boolean;
  showPassword: boolean = false;
  showUser: boolean = false;
  showInfo: boolean = true;
  loaded: boolean = false;
  errorUnique: boolean = false;
  errorUsername: boolean = false;

  editForm: FormGroup;
  editUsernameForm: FormGroup;
  editPasswordForm: FormGroup;
  showError: boolean;
  

  constructor(private service: UserService,
              private fb: FormBuilder) { 
    this.activeUser = new User();
  }

  ngOnInit() {
    this.getUserInfo();

    this.editForm = this.fb.group({
      // username: ['',Validators.required],
      fullname: ['',Validators.required],
      gender: [''],
      jmbg: [''],
      contactNumber:[''],
    });

    this.editUsernameForm = this.fb.group({
      username: ['',Validators.required]
    });

    this.editPasswordForm = this.fb.group({
      oldPassword: ['',Validators.required],
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

  }

  ngAfterViewInit()
  {
    this.editUsernameForm.controls['username'].valueChanges.subscribe(
      data =>{
        this.submitted = false;
      });

    this.editPasswordForm.valueChanges.subscribe(
      data =>{
        this.submitted = false;
      });
  }

  get f() { return this.editForm.controls; }
  get u() { return this.editUsernameForm.controls; }
  get p() { return this.editPasswordForm.controls; }

  OnSubmit()
  {
    debugger
    this.submitted = true;
    
    // stop here if form is invalid
    if (this.editForm.invalid) {
        return;
    }

    this.editedUser  = this.activeUser;
    // this.editedUser.Username = this.editForm.controls['username'].value;
    this.editedUser.FullName = this.editForm.controls['fullname'].value;
    this.editedUser.Gender = this.editForm.controls['gender'].value;
    this.editedUser.JMBG = this.editForm.controls['jmbg'].value;
    this.editedUser.ContactNumber = this.editForm.controls['contactNumber'].value;

    this.service.UpdateUser(this.editedUser).subscribe(
      data=>{
        debugger
        this.Cancel();
        this.loaded = false;
        this.getUserInfo();
      },
      error=>{
          this.showError = true;
          this.submitted = false;
      }
    )


  }

  OnSubmitUsername(){
    this.submitted = true;
    debugger
    // stop here if form is invalid
    if (this.editUsernameForm.invalid) {
      return;
    }

    this.tempUser = this.activeUser;
    this.tempUser.Username = this.editUsernameForm.controls['username'].value;    

    this.service.UpdateUser(this.tempUser).subscribe(
      data =>{
        this.editUsernameForm.reset();
        this.submitted = false;
        this.showInfo = true;
        this.showUser = false;
      },
      error=>{
        var a = this.activeUser;
        if(error.error.Message = "Username not Unique"){
          this.errorUnique = true;
        }
        else{
          this.errorUsername = true;
        }
        this.submitted = false;
        this.activeUser.Username = this.oldUsername;
      }
    )

  }

  OnSubmitPassword(){
    this.submitted = true;
    debugger
    // stop here if form is invalid
    if (this.editPasswordForm.invalid) {
      return;
    }

    this.service.ChangePassword(this.editPasswordForm.value).subscribe(
      data => {
        this.Cancel();
      },
      error => {
        this.showError = true;
      }

    )

  }

  getUserInfo(){
    this.service.getUser().subscribe(
      data=>{
        var res = data;
        this.activeUser = res;
        this.loaded = true;
        if(this.activeUser.Role == "0")
          this.activeUser.Role = "User"
        else if(this.activeUser.Role == "1")
          this.activeUser.Role = "Driver"
        else if(this.activeUser.Role == "2")
          this.activeUser.Role = "Admin"
      }
      );
  }

  EditUser()
  {
    this.showInfo = false;
    this.showUser = true;
    this.oldUsername = this.activeUser.Username;
  }

  EditPassword()
  {
    this.showInfo = false;
    this.showPassword = true;
  }
  
  Edit(){
    this.showInfo = false;
    this.showEdit = true;
    debugger
    // this.editForm.controls['username'].setValue(this.activeUser.Username, {onlySelf: true});
    this.editForm.controls['fullname'].setValue(this.activeUser.FullName, {onlySelf: true});
    this.editForm.controls['gender'].setValue(this.activeUser.Gender, {onlySelf: true});
    this.editForm.controls['jmbg'].setValue(this.activeUser.JMBG, {onlySelf: true});
    this.editForm.controls['contactNumber'].setValue(this.activeUser.ContactNumber);
  //   this.editForm.setValue({
  //     username: this.activeUser.Username,
  //     fullname: this.activeUser.FullName,
  //     email: this.activeUser.Email,
  //     gender: this.activeUser.Gender,
  //     jmbg: this.activeUser.JMBG,
  //     contactNumber: this.activeUser.ContactNumber      
  //   });
  
  }

  Cancel()
  {
    this.showInfo = true;
    this.showEdit = false;
    this.editForm.reset();
    this.editPasswordForm.reset();
    this.editUsernameForm.reset();
    this.showPassword = false;
    this.showUser = false;
    this.errorUnique =false;
    this.errorUsername = false;
  }



}
