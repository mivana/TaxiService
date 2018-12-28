import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
 
import { AppSettings } from '../app.settings'
import { Observable } from 'rxjs/internal/Observable';
import { retry } from 'rxjs/internal/operators/retry';
import { User } from '../models/User.model';
import { Variable } from '@angular/compiler/src/render3/r3_ast';
import { templateJitUrl } from '@angular/compiler';
import { AppUserAuthGuard } from '../guards/appUser.guard';
import { Password } from '../models/Password.model';
 
@Injectable()
export class UserService {
    constructor(private http: HttpClient) { }

    register(user: User, role: string) {
        user.Role = role;
        return this.http.post(AppSettings.API_ENDPOINT+'/api/User/Register', user);
    }

    getUser() : Observable<any>
    {
        return this.http.get(AppSettings.API_ENDPOINT + '/api/User/GetActiveInfo');
    }

    UpdateUser(user: User) : Observable<any>{
        return this.http.put(AppSettings.API_ENDPOINT + '/api/User/'+user.Id, user);
    }

    UpdateUsername(id: string, username: User): Observable<any> {
        return this.http.put(AppSettings.API_ENDPOINT + '/api/User/UpdateUsername/'+id, username);
    }

    ChangePassword(password: Password): Observable<any> {
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Account/ChangePassword',password);
    }


}