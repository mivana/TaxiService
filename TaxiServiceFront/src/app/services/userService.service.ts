import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
 
import { AppSettings } from '../app.settings'
import { Observable } from 'rxjs/internal/Observable';
import { User } from '../models/User.model';
import { Password } from '../models/Password.model';
import { RegisterDriver } from '../models/RegisterDriver.model';
 
@Injectable()
export class UserService {
    constructor(private http: HttpClient) { }

    register(user: User, role: string) {
        user.Role = role;
        return this.http.post(AppSettings.API_ENDPOINT+'/api/User/Register', user);
    }

    registerDriver(regDriver: RegisterDriver, role: string) {
        regDriver.Role = role;
        return this.http.post(AppSettings.API_ENDPOINT+'/api/User/RegisterDriver', regDriver);
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