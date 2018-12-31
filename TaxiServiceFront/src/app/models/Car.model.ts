import { User } from './User.model';

export class Car {
    Id: string;
    TaxiNumber: string;
    YearMade: string;
    RegistrationPlate: string;
    CarType: string;
    Deleted: Boolean;
    Driver: User;
}