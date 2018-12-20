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

@NgModule({
  declarations: [
    AppComponent,
    MenuComponent,
    HomeComponent,
    UserHomeComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: JwtInterceptor,
            multi: true
        },
    AuthenticationService,
    SessionService,
    AppUserAuthGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
