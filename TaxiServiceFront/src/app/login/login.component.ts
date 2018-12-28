import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from '../services/authenticationservice.service';
import { SessionService } from '../services/sessionservice.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  submitted= false;
  loginForm: FormGroup;
  showError: boolean = false;
  showResult: boolean = false;

  constructor(private fb: FormBuilder,
              private authService: AuthenticationService,
              private router: Router) { }

  ngOnInit() {
    this.loginForm = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  ngAfterViewInit()
    {
      this.loginForm.valueChanges.subscribe(
        data =>{
          this.submitted = false;
        }
      )
    }


   // convenience getter for easy access to form fields
   get f() { return this.loginForm.controls; }

   onSubmit() {
    this.submitted = true;

    this.showError = false;
    this.showResult = false;

    // stop here if form is invalid
    if (this.loginForm.invalid) {
        return;
    }

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

          if(SessionService.isAdmin())
          {
              this.router.navigate(['/adminDashboard']);
          }
          else
          {
              this.router.navigate(['/userHome']);
          }

      },
      error => {
            console.log("Error occured");
            if(error.error.error_description == "The user name or password is incorrect.!!!!")
                this.showResult = true;
            else   
                this.showError = true;
            this.submitted = false;
        });
  }

  


}