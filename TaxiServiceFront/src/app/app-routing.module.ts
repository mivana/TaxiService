import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { UserHomeComponent } from './user-home/user-home.component';
import { AppUserAuthGuard } from './guards/appUser.guard';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { AdminAuthGuard } from './guards/admin.guard';
import { DriverAuthGuard } from './guards/driver.guard';
import { MenuComponent } from './menu/menu.component';
import { AllUsersGuard } from './guards/allUsers.guard';

export const routes: Routes = [
  {
    path: "",
    redirectTo: "register",
    pathMatch: "full"
  },
  {
    path: "menu",
    component: MenuComponent,
  },
  {
    path: "register",
    component: RegisterComponent
  },
  {
    path: "home",
    component: HomeComponent
  },
  {
    path: "userHome",
    component:UserHomeComponent,
    canActivate: [AppUserAuthGuard]
  },
  {
    path: "myProfile",
    component: MyProfileComponent,
    canActivate: [AllUsersGuard]
  },
  {
    path: "login",
    component: LoginComponent,
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
