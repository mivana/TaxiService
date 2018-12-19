import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthenticationService } from '../services/authenticationservice.service';
 
@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(public auth: AuthenticationService) { }
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        console.log("Intercepted");
        console.log("Token : ", localStorage.jwt);

        let jwt = localStorage.jwt;

        if (jwt) 
        {
            console.log(request)
            request = request.clone(
                {
                    setHeaders: 
                    { 
                        Authorization: `Bearer ${jwt}`
                    }
                });
            console.log(request)
        }

        return next.handle(request);

    }
}