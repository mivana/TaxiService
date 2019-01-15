import { Injectable, QueryList } from '@angular/core';
import { HttpClient } from '@angular/common/http';
 
import { Observable } from 'rxjs';
 
@Injectable()
export class SessionService {
    constructor() { }
 
    public static isAdmin():boolean
    {
        return localStorage.role == "Admin";
    }

    public static isDriver():boolean
    {
        return localStorage.role == "Driver";
    }

    public static isntLoggedOn():boolean
    {
        return localStorage.role != "AppUser" && localStorage.role != "Driver";
    }

    public static isUser():boolean
    {
        return localStorage.role == "AppUser";
    }
    
}