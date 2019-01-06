import { User } from './User.model';
import { Ride } from './Ride.model';

export class Comment {
    Id: string;
    Content: string;
    Rating: string;
    DateCreated: string;
    AppUser: User;
    AppUserID: string
    Ride: Ride;
    RideID: string;
    Deleted: boolean;
}