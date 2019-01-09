import { Car } from './Car.model';
import { Comment } from './Comment.model';

export class User {
    Id: string;
    Email: string;
    FullName: string;
    Username: string;
    JMBG: string;
    ContactNumber: string;
    DriverFree: boolean;
    Deleted: boolean;
    DriverLocationId: string;
    Role: string;
    Gender: string;
    Blocked: boolean;
    DriverCars: Car[];
}