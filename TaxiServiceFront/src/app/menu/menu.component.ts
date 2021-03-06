import { Component, OnInit } from '@angular/core';
import { SessionService } from '../services/sessionservice.service';
import { AuthenticationService } from '../services/authenticationservice.service';
import { Router } from '@angular/router';
import { Session } from 'protractor';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css'],
  providers: [AuthenticationService]
})
export class MenuComponent implements OnInit {

  constructor(private authService: AuthenticationService,
              private router: Router) { }

  ngOnInit() {
  }

  LogOut(){
    this.authService.logout();
    this.router.navigate(['/register']);
  }

  isntLoggedOn()
  {
      return SessionService.isntLoggedOn();
  }
  
  isAdmin()
  {
    return SessionService.isAdmin();
  }

  isDriver()
  {
    return SessionService.isDriver();
  }
}
