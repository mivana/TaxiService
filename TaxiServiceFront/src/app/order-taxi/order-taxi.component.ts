import { Component, OnInit } from '@angular/core';
import { AppUserAuthGuard } from '../guards/appUser.guard';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';
import { AdminAppUserAuthGuard } from '../guards/adminAppUser.guard';

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

  constructor() { }

  ngOnInit() {
  }

}
