import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
 
import { AppSettings } from '../app.settings'
import { Observable } from 'rxjs/internal/Observable';
import { User } from '../models/User.model';
import { Password } from '../models/Password.model';
import { RegisterDriver } from '../models/RegisterDriver.model';
import { NewRide } from '../models/NewRide.model';
import { Ride } from '../models/Ride.model';
import { Comment } from '../models/Comment.model';
import { Car } from '../models/Car.model';
import { TakeRide } from '../models/TakeRide.model';
import { TouchSequence } from 'selenium-webdriver';
import { FinishRide } from '../models/FinishRide.model';
import { CommentRide } from '../models/CommentRide.model';
import { AppUserAuthGuard } from '../guards/appUser.guard';
import { Address } from '../models/Address.model';
import { Search } from '../models/Search.Model';
import { AssignDriver } from '../models/AssignDriver.model';
import { SearchAdmin } from '../models/SearchAdmin.Model';
import { SortModel } from '../models/Sort.Model';
 
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

    getRole(): Observable<any>{
        return this.http.get(AppSettings.API_ENDPOINT + '/api/User/GetUserRole');
    }

    getDrivers(): Observable<any> {
        return this.http.get(AppSettings.API_ENDPOINT + '/api/User/GetFreeDrivers');
    }

    GetFreeRides():Observable<any>{
        return this.http.get(AppSettings.API_ENDPOINT +'/api/Ride/GetFreeRides')
    }

    GetRides():Observable<any>{
        return this.http.get(AppSettings.API_ENDPOINT + '/api/Ride');
    }

    UpdateUsername(id: string, username: User): Observable<any> {
        return this.http.put(AppSettings.API_ENDPOINT + '/api/User/UpdateUsername/'+id, username);
    }

    ChangePassword(password: Password): Observable<any> {
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Account/ChangePassword',password);
    }

    PostRide(ride: NewRide): Observable<any> {
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Ride/PostNewRide',ride);        
    }

    PutRide(id: string, ride: Ride): Observable<any> {
        return this.http.put(AppSettings.API_ENDPOINT + '/api/Ride/'+id,ride);
    }

    PutEditRide(id: string, ride: Ride): Observable<any> {
        return this.http.put(AppSettings.API_ENDPOINT + '/api/Ride/'+id,ride);
    }

    AddComment(comment: Comment): Observable<any> {
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Ride/AddComment',comment);
    }

    CancelRideComplete(cancelRide: CommentRide): Observable<any> {
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Ride/CancelRideComplete',cancelRide);
    }

    UpdateCar(id: string, car: Car): Observable<any>{
        return this.http.put(AppSettings.API_ENDPOINT + '/api/Car/'+id,car);
    }

    TakeRide(idRide: string, takeRide: Ride): Observable<any> {
        return this.http.put(AppSettings.API_ENDPOINT + '/api/Ride/TakeRide/'+idRide,takeRide);
    }

    FinishRide(finishRide: FinishRide): Observable<any> {
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Ride/FinishRide',finishRide);
    }

    CommentRideComplete(commentRide: CommentRide): Observable<any> {
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Ride/CommentRideComplete',commentRide);
    }

    ChangeDriverLocation(address: Address): Observable<any>{
        return this.http.post(AppSettings.API_ENDPOINT + '/api/User/ChangeDriverLocation',address);
    }

    GetAddress(idAddress: string):Observable<any>{
        return this.http.get(AppSettings.API_ENDPOINT + '/api/User/GetAddress/'+idAddress);
    }

    Search(search: Search):Observable<any>{
        if(search.Created == null)
            search.Created = false;
        if(search.Accepted == null)
            search.Accepted = false;
        if(search.Proccessed == null)
            search.Proccessed = false;
        if(search.Failed == null)
            search.Failed = false;
        if(search.Formed == null)
            search.Formed = false;
        if(search.Successfull == null)
            search.Successfull = false;
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Ride/Search',search);
    }

    SearchAdmin(search: SearchAdmin):Observable<any>{
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Ride/SearchAdmin',search);
    }

    AssignDriver(assignDriver: AssignDriver, ride: Ride){
        assignDriver.Ride = ride;
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Ride/AssignDriver',assignDriver);
    }

    Sort(sortBy: SortModel):Observable<any>{
        return this.http.post(AppSettings.API_ENDPOINT + '/api/Ride/Sort',sortBy);
    }
}