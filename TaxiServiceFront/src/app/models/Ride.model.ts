import { User } from "./user.model";
import { Location } from './Location.model';

export class Ride {
    Id: string;
    Status: string;
    OrderDT: string;
    Price: string;
    Deleted: string;
    StartLocation: Location;
    StartLocationID: string;
    DestinationLocation: Location;
    DestinationLocationID: string;
    CarType: string;
    AppUserID: string;
    // Customer: User;
    DispatcherID: string;
    Dispatcher: User;
    TaxiDriverID: string;
    TaxiDriver: User;
}