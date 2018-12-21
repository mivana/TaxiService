import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
 
import { AppSettings } from '../app.settings'
import { Observable } from 'rxjs/internal/Observable';
import { retry } from 'rxjs/internal/operators/retry';
import { User } from '../models/User.model';
 
@Injectable()
export class UserService {
    constructor(private http: HttpClient) { }

    register(user: User, role: string) {
        user.Role = role;
        return this.http.post(AppSettings.API_ENDPOINT+'/api/Account/Register', user);
    }


}