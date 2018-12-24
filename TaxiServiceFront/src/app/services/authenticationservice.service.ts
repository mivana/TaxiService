import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AppSettings } from '../app.settings';
import { Router } from '@angular/router';
import { HomeComponent } from '../home/home.component';
import { SessionService } from './sessionservice.service';

 
@Injectable()
export class AuthenticationService {
    constructor(private http: HttpClient, private router: Router) { }
 
    login(email: string, password: string) {
        if(!localStorage.jwt)
        {
            let headers = new HttpHeaders();
            headers = headers.append('Content-type', 'application/x-www-form-urlencoded');
            
            return this.http.post
            (AppSettings.API_ENDPOINT+'/oauth/token','username='+email+'&password='+password+'&grant_type=password',
            {"headers": headers}) as Observable<any>
            
           /* let x = this.http.post<any>(AppSettings.API_ENDPOINT+'/oauth/token',
            { email: email, password: password, grant_type: 'AppUser' })*/



            /* let x = this.http.get(AppSettings.API_ENDPOINT+'/api/Services') as Observable<any>

            x.subscribe(
                res => {
                console.log(res);
                },
                err => {
                console.log("Error occured");
                }
            );  */
    }
}
 
    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('jwt');
        localStorage.removeItem('role');
    }
}