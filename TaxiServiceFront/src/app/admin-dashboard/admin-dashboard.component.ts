import { Component, OnInit } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';
import { UserService } from '../services/userService.service';
import { User } from '../models/User.model';
import { Ride } from '../models/Ride.model';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css'],
  providers:[
    AdminDashboardComponent,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
  ]
})
export class AdminDashboardComponent implements OnInit {
  showAll: boolean = true;
  showMy: boolean = false;
  showFree: boolean = false;
  hasError: boolean;
  showAssign: boolean = false;

  errorString: string = "";
  
  
  activeUser: User = new User();
  tempRides: Ride[] =[];
  myRides: Ride[] =[];
  freeRides: Ride[] =[];
  allRides: Ride[] =[];
  assignRide: Ride = new Ride();
  freeDrivers: User[] = [];
  CTSdrivers: User[] = [];
  CTCdrivers: User[] = [];
  drivers: User[] = [];

  assignForm: FormGroup;
  submitted: boolean;


  constructor(private service: UserService,
              private fb: FormBuilder) { }

  ngOnInit() {
    this.assignForm = this.fb.group({
      driver: ['']
    })
    this.GetAll();
  }

  ngAfterViewInit()
  {
    this.assignForm.valueChanges.subscribe(
      data =>{
        this.submitted = false;
      }
    )

  }

  GetInfo(){
    this.service.getUser().subscribe(
      data=>{
        debugger
        var user = data;
        this.activeUser = user;
        this.tempRides = user.DispatcherRides;
        this.myRides = this.tempRides.reverse();
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
    } 
    )
  }

  GetFree(){
    this.service.GetFreeRides().subscribe(
      data=>{
        debugger
        this.freeRides = data;
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
    )
  }

  GetAll(){
    this.service.GetRides().subscribe(
      data=>{
        debugger
        this.tempRides = data;
        this.allRides = this.tempRides.reverse();
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
    )
  }

  getDrivers(){
    this.drivers = [];
    this.CTSdrivers = [];
    this.CTCdrivers = [];
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
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
        this.showAssign = false;
      }
    )
  }

  get f() { return this.assignForm.controls; }

  OnSubmit(){
    this.submitted = true;
    this.hasError = false;
    this.errorString = "";

    debugger
    if(this.assignForm.controls['driver'].value == "")
        this.assignForm.controls['driver'].setErrors({required: true});
      
    // stop here if form is invalid
    if (this.assignForm.invalid) {
      return;
    }

    this.service.AssignDriver(this.assignForm.value, this.assignRide).subscribe(
      data => {
        this.showAssign = false;
        this.GetFree();
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
        this.showAssign = false;
      }
    )

  }




  All(){
    this.showAll = true;
    this.showMy = false;
    this.showFree = false;
    this.hasError = false;
    this.errorString = "";
    this.GetAll();
  }

  My(){
    this.showAll = false;
    this.showMy = true;
    this.showFree = false;
    this.hasError = false;
    this.errorString = "";
    this.GetInfo();
  }

  Free(){
    this.showAll = false;
    this.showMy = false;
    this.showFree = true;
    this.hasError = false;
    this.errorString = "";
    this.GetFree();
  }

  Assign(ride:Ride){
    this.showAssign = true;
    this.assignRide = ride;
    this.getDrivers();
    if(this.assignRide.CarType == "0")
      this.drivers = this.CTSdrivers
    else
      this.drivers = this.CTCdrivers;
  }

  Cancel(){
    this.assignRide = null;
    this.showAssign = false;
    this.hasError = false;
    this.errorString = "";
    

  }

}
