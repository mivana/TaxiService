import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { UserHomeComponent } from './user-home/user-home.component';
import { AppUserAuthGuard } from './guards/appUser.guard';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { AdminAuthGuard } from './guards/admin.guard';
import { DriverAuthGuard } from './guards/driver.guard';
import { MenuComponent } from './menu/menu.component';
import { AllUsersGuard } from './guards/allUsers.guard';
import { OrderTaxiComponent } from './order-taxi/order-taxi.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AddDriverComponent } from './add-driver/add-driver.component';
import { AdminAppUserAuthGuard } from './guards/adminAppUser.guard';
import { DriverHomeComponent } from './driver-home/driver-home.component';
import { SearchComponent } from './search/search.component';

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
  },
  {
    path: "orderTaxi",
    component: OrderTaxiComponent,
    canActivate: [AdminAppUserAuthGuard]
  },
  {
    path: "adminDashboard",
    component: AdminDashboardComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: "addDriver",
    component: AddDriverComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: "driverHome",
    component: DriverHomeComponent,
    canActivate: [DriverAuthGuard]
  },
  {
    path: "search",
    component: SearchComponent,
    canActivate: [AllUsersGuard],
  }
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
