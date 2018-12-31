import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MenuComponent } from './menu/menu.component';
import { HomeComponent } from './home/home.component';
import { AuthenticationService } from './services/authenticationservice.service';
import { SessionService } from './services/sessionservice.service';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { UserHomeComponent } from './user-home/user-home.component';
import { JwtInterceptor } from './helper/jwt.interceptor';
import { AppUserAuthGuard } from './guards/appUser.guard';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { UserService } from './services/userService.service';
import { AdminAuthGuard } from './guards/admin.guard';
import { DriverAuthGuard } from './guards/driver.guard';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { AllUsersGuard } from './guards/allUsers.guard';
import { OrderTaxiComponent } from './order-taxi/order-taxi.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AddDriverComponent } from './add-driver/add-driver.component';
import { AdminAppUserAuthGuard } from './guards/adminAppUser.guard';

@NgModule({
  declarations: [
    AppComponent,
    MenuComponent,
    HomeComponent,
    UserHomeComponent,
    LoginComponent,
    RegisterComponent,
    MyProfileComponent,
    OrderTaxiComponent,
    AdminDashboardComponent,
    AddDriverComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: JwtInterceptor,
            multi: true
        },
    AuthenticationService,
    SessionService,
    UserService,
    AppUserAuthGuard,
    AdminAuthGuard,
    DriverAuthGuard,
    AllUsersGuard,
    AdminAppUserAuthGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
