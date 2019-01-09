import { Component, OnInit } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';
import { UserService } from '../services/userService.service';
import { User } from '../models/User.model';
import { Ride } from '../models/Ride.model';

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

  errorString: string = "";
  
  
  activeUser: User = new User();
  tempRides: Ride[] =[];
  myRides: Ride[] =[];
  freeRides: Ride[] =[];
  allRides: Ride[] =[];

  constructor(private service: UserService) { }

  ngOnInit() {
    this.GetAll();
  }

  /*
    Onemoguci ako je driver zauzet da se prikazuje pri porucivanja novog vozila kod admina
    Dispacher ime se nesto ne vidi na myhistory
    Promena lokacije vozaca, kada krene voznju, da mu je to trenutna lokacija, i da moze samostano da menja svoju lokaciju
    i onda je ostalo ono PFS. Good luck! <3
  */

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

  All(){
    this.showAll = true;
    this.showMy = false;
    this.showFree = false;
    this.GetAll();
  }

  My(){
    this.showAll = false;
    this.showMy = true;
    this.showFree = false;
    this.GetInfo();
  }

  Free(){
    this.showAll = false;
    this.showMy = false;
    this.showFree = true;
    this.GetFree();

  }


}
