import { Component, OnInit } from '@angular/core';
import { DriverAuthGuard } from '../guards/driver.guard';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';
import { FormBuilder } from '@angular/forms';
import { UserService } from '../services/userService.service';
import { User } from '../models/User.model';
import { Ride } from '../models/Ride.model';

@Component({
  selector: 'app-driver-home',
  templateUrl: './driver-home.component.html',
  styleUrls: ['./driver-home.component.css'],
  providers:[
    DriverAuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
  ]
})
export class DriverHomeComponent implements OnInit {
  activeUser: User = new User();
  myRides: Ride[] = [];
  freeRides: Ride[] = [];


  hasError: boolean;
  errorString: string = "";
  showFree: boolean;
  showHistory: boolean;
  myCarType: string;

  constructor(private fb: FormBuilder,
              private service: UserService) { }

  ngOnInit() {
    this.GetUserInfo();
    this.GetFreeRides();
  }
  GetFreeRides(): any {
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

   ///Gets Users Information
   GetUserInfo(){
    this.service.getUser().subscribe(
      data => {
        debugger
        var user = data;
        this.activeUser = user;
        this.myRides = user.DriverRides;
        this.myCarType = this.activeUser.DriverCars[0].CarType;

      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
    )
  }

  History(){
    this.showHistory = true;
    this.showFree = false;
  }

  Created(){
    this.showFree = true;
    this.showHistory = false;
  }


}
