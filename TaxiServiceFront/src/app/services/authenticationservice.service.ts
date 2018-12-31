import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppSettings } from '../app.settings';
import { Router } from '@angular/router';

 
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

            // x.subscribe(
            //     res => {
            //         console.log(res.access_token);
          
            //         let jwt = res.access_token;

            //         let jwtData = jwt.split('.')[1]
            //         let decodedJwtJsonData = window.atob(jwtData)
            //         let decodedJwtData = JSON.parse(decodedJwtJsonData)

            //         let role = decodedJwtData.role

            //         console.log('jwtData: ' + jwtData)
            //         console.log('decodedJwtJsonData: ' + decodedJwtJsonData)
            //         console.log('decodedJwtData: ' + decodedJwtData)
            //         console.log('Role ' + role)

            //         localStorage.setItem('jwt', jwt)
            //         localStorage.setItem('role', role);

            //         if(SessionService.isAdmin())
            //         {
            //             this.router.navigate(['/adminDashboard']);
            //         }
            //         else
            //         {
            //             this.router.navigate(['/userHome']);
            //         }

            //     },
            //     err => {
            //         console.log("Error occured");
            //     });
            // }

            


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