import { Component, OnInit } from '@angular/core';
import { AppUserAuthGuard } from '../guards/appUser.guard';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';
import { Ride } from '../models/Ride.model';
import { UserService } from '../services/userService.service';

@Component({
  selector: 'app-user-home',
  templateUrl: './user-home.component.html',
  styleUrls: ['./user-home.component.css'],
  providers:[
    AppUserAuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
  ]
})
export class UserHomeComponent implements OnInit {

  myRides: Ride[] = [];
  constructor(private service: UserService) { }

  ngOnInit() {
    this.GetUserInfo();
  }

  GetUserInfo(){
    this.service.getUser().subscribe(
      data => {
        debugger
        var user = data;
        this.myRides = user.CustomerRides;
      }
    )
  }

}
