import { Component, OnInit } from '@angular/core';
import { AppUserAuthGuard } from '../guards/appUser.guard';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';
import { AdminAppUserAuthGuard } from '../guards/adminAppUser.guard';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { UserService } from '../services/userService.service';
import { NumbericValidation } from '../validators/numeric-validator.validation';
import { Router } from '@angular/router';
import { User } from '../models/User.model';
import { Ride } from '../models/Ride.model';

@Component({
  selector: 'app-order-taxi',
  templateUrl: './order-taxi.component.html',
  styleUrls: ['./order-taxi.component.css'],
  providers:[
    AdminAppUserAuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
  ]
})
export class OrderTaxiComponent implements OnInit {
  orderForm: FormGroup;
  
  submitted: boolean = false;
  errorNumber: boolean = false;
  errorAreaCode: boolean = false;
  result: boolean = false;
  roleAdmin: boolean = false;
  hasError: boolean = false;
  isStandard: boolean = true;
  errorString: string = "";
  hasActive: boolean = false;

  carTypes: string[] = ['Standard','Combi'];
  default: string = 'Standard';

  drivers: User[] = [];
  freeDrivers: User[] = [];
  CTSdrivers: User[] = [];
  CTCdrivers: User[] = [];
  activeUser: User = new User();
  myRides: Ride[] = [];
  
  
  constructor(private fb: FormBuilder,
              private service: UserService,
              private router: Router) { }

  ngOnInit() {
    this.orderForm = this.fb.group({
      streetName: ['',Validators.required],
      number: ['',[Validators.required,NumbericValidation.numberValidator]],
      town: ['',Validators.required],
      areaCode:['',[Validators.required,NumbericValidation.numberValidator]],
      carType: [null],
      driver: ['']
    });
    this.orderForm.controls['carType'].setValue(this.default, {onlySelf: true});
    this.getRole();
    this.GetUserInfo();
  }

  ngAfterViewInit()
  {
    this.orderForm.valueChanges.subscribe(
      data =>{
        this.submitted = false;
        this.result = false;
        debugger
        if(this.orderForm.controls['carType'].value == "Standard"){
          this.isStandard = true;
          this.freeDrivers = this.CTSdrivers;
        }
        else{
          this.isStandard = false;
          this.freeDrivers = this.CTCdrivers;
        }
      }
    )

  }

  GetUserInfo(){
    this.service.getUser().subscribe(
      data => {
        var user = data;
        this.activeUser = user;
        if(this.activeUser.Role == "0"){
          this.myRides = user.CustomerRides;
          var i = this.myRides.length;
          if(this.myRides[i-1] != null && this.myRides[i-1].Status != "4"){
            if(this.myRides[i-1].Status != "5"){
              if(this.myRides[i-1].Status != "6"){
                this.hasActive = true;
              }
            }
          }
        }   
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
      )
  }

  getRole(){
    this.service.getRole().subscribe(
      data => {
        var temp = data;
        if(temp == "2"){
          this.roleAdmin = true;
          this.getDrivers();
        }
        else
          this.roleAdmin = false;
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
    )
  }

  getDrivers(){
    this.service.getDrivers().subscribe(
      data => {
        debugger
        var temp = data;
        this.drivers = temp;

        for(var i = 0; i < this.drivers.length;i++)
        {
          if(this.drivers[i].DriverCars[0].CarType == "0")
            this.CTSdrivers.push(this.drivers[i]);
          else
            this.CTCdrivers.push(this.drivers[i]);
        }

        this.freeDrivers = this.CTSdrivers;
        // this.drivers.forEach(element => {
        //   if(element.DriverCars[0].CarType == "0")
        //   {
        //     this.CTSdrivers.push(element);
        //   }
        //   else
        //   {
        //     this.CTCdrivers.push(element);
        //   }
        // });
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
    )
  }

  get f() { return this.orderForm.controls; }

  OnSubmit(){
    this.submitted = true;
    this.hasError = false;
    this.errorString = "";

    debugger
    if(this.roleAdmin && this.orderForm.controls['driver'].value == "")
        this.orderForm.controls['driver'].setErrors({required: true});
      
   // stop here if form is invalid
   if (this.orderForm.invalid) {
    return;
   }


  this.service.PostRide(this.orderForm.value).subscribe(
    data => {
      this.result = true;
      this.getRole();
      this.GetUserInfo();
    },
    error => {
      this.hasError = true;
      this.errorString = error.error.Message;
    }
  )   

  }

  IsStandard(){
    if(this.orderForm.controls['carType'].value == "Standard")
      return true;
    else
      return false;
  }


  Cancel(){
    this.submitted = false;
    this.errorAreaCode = false;
    this.errorNumber = false;
    this.hasError = false;
    this.errorString = "";

    this.orderForm.reset();

  }

/*
  Za dodavanje ovoga,
  isprobaj servis da li ce da primi ok putanju,
  stavi validaciju da li je int sa ove strane!
  Sa back-a, u slucaju da se desi greska neka da revert-uje sve akcije koje je on 
    upravo uradio! OBAVEZNO

  prikazi svih gresaka!


*/

}