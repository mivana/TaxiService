import { User } from './User.model';

export class NewRide {
    StreetName: string;
    Number: string;
    Town: string;
    AreaCode: string;
    CarType: string;
    Driver: User;
}