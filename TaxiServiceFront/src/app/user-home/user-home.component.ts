import { Component, OnInit } from '@angular/core';
import { AppUserAuthGuard } from '../guards/appUser.guard';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';

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

  constructor() { }

  ngOnInit() {
  }

}
