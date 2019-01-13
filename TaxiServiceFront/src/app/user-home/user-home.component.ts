import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { AppUserAuthGuard } from '../guards/appUser.guard';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';
import { Ride } from '../models/Ride.model';
import { UserService } from '../services/userService.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NumbericValidation } from '../validators/numeric-validator.validation';
import { Comment } from '../models/Comment.model';
import { User } from '../models/User.model';
import { CommentRide } from '../models/CommentRide.model';

@Component({
  selector: 'app-user-home',
  templateUrl: './user-home.component.html',
  styleUrls: ['./user-home.component.css'],
  providers:[
    AppUserAuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
  ]
})
export class UserHomeComponent implements OnInit {
  EditForm: FormGroup;
  CommentForm: FormGroup;

  myRides: Ride[] = [];
  activeRide: Ride = new Ride();
  cancelledRide: Ride = new Ride();
  userComment: Comment = new Comment();
  cancelRide: CommentRide = new CommentRide();
  activeUser: User = new User();

  carTypes: string[] = ['Standard','Combi'];
  default: string = 'Standard';

  hasActive: boolean = false;
  showEdit: boolean = false;
  submitted: boolean;
  hasError: boolean;
  hasErrorUser: boolean = false;
  noActive: boolean = true;
  errorString: string = "";
  result: boolean;
  enableEdit: boolean = false;
  showComment: boolean = false;

  @Input() rating: number = 0;
  @Input() itemId: number;
  @Output() ratingClick: EventEmitter<any> = new EventEmitter<any>();

  constructor(private service: UserService,
              private fb: FormBuilder) { }

  ngOnInit() {
    this.EditForm = this.fb.group({
      streetName: ['', Validators.required],
      number: ['', [Validators.required,NumbericValidation.numberValidator]],
      town: ['', Validators.required],
      areaCode: ['',[Validators.required,NumbericValidation.numberValidator]],
      carType: ['']
    })
    this.CommentForm = this.fb.group({
      content: ['', Validators.required]
    });

    this.EditForm.controls['carType'].setValue(this.default, {onlySelf: true});


    this.GetUserInfo();
  }

  ///When input changes, buttons are not disabled anymore
  ngAfterViewInit()
  {
    this.EditForm.valueChanges.subscribe(
      data =>{
        this.submitted = false;
        this.result = false;
      }
    )

    this.CommentForm.valueChanges.subscribe(
      data =>{
        this.submitted = false;
        this.result = false;
      }
    )
    
  }

  ///Gets Users Information, and retreives user's history
  GetUserInfo(){
    this.service.getUser().subscribe(
      data => {
        debugger
        var user = data;
        this.activeUser = user;
        this.myRides = user.CustomerRides;
        var i = this.myRides.length;
        if(this.myRides[i-1] != null && this.myRides[i-1].Status != "1"){
          this.activeRide = this.myRides[i-1];
          this.hasActive = true;
          this.noActive = false;
          if(this.activeRide.Status == "6" && this.activeRide.UserComment[0] == null)
            this.showComment = true;
        }
        else
        {
          this.noActive = true;
          this.hasActive = false;
        }

        if(this.myRides[i-1].Status != "0")
          this.enableEdit = true;
        

        this.myRides.reverse();

      },
      error => {
        this.hasErrorUser = true;
        this.errorString = error.error.Message;
      }
    )
  }

  ///returns controls of Forms
  get f() { return this.EditForm.controls; }
  get c() { return this.CommentForm.controls; }

  ///Submits Edit Form and Edits active ride
  OnSubmit(){
    this.submitted = true;
    this.hasError = false;
    this.errorString = "";

      
   // stop here if form is invalid
   if (this.EditForm.invalid) {
    return;
   }

   this.activeRide.StartLocation.Address.StreetName = this.EditForm.controls['streetName'].value;
   this.activeRide.StartLocation.Address.Number = this.EditForm.controls['number'].value;
   this.activeRide.StartLocation.Address.Town = this.EditForm.controls['town'].value;
   this.activeRide.StartLocation.Address.AreaCode = this.EditForm.controls['areaCode'].value;
   this.activeRide.CarType = this.EditForm.controls['carType'].value;

   this.service.PutEditRide(this.activeRide.Id,this.activeRide).subscribe(
     data =>{
      this.showEdit = false;
      this.hasActive = true;
      this.submitted = false;
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
     );
  }

  ///Shows Comment form when you want to cancel active ride
  Cancel(ride: Ride){
    // var r = ride;
    // r.Deleted = true;
    // r.Status = "1";
  
    this.hasActive = false;
    this.noActive= false;
    this.showComment = true;
   
    // this.service.PutRide(r.Id,r).subscribe(
    //   data => {
    //     this.activeRide.Status = "1";
    //     this.activeRide.Deleted = true;
    //     this.cancelledRide = this.activeRide;
    //     this.activeRide = null;
    //     this.hasActive = false;
    //     this.noActive= true;
    //     this.showComment = true;

    //   }
    // )
  }

  ///For rating
  onClick(rating: number): void {
    this.rating = rating;
    this.ratingClick.emit({
      itemId: this.itemId,
      rating: rating
    });
  }

  ///Submits Comment Form, Cancels user's ride, and post user's comment
  OnSubmitComment(){
    debugger
      this.submitted = true;
      this.hasError = false;
      this.errorString = "";
        
     // stop here if form is invalid
     if (this.CommentForm.invalid) {
      return;
     }
     
     this.userComment.Content = this.CommentForm.controls['content'].value;
     this.userComment.Rating = this.rating.toString();
     this.userComment.RideID = this.cancelledRide.Id;
     this.userComment.Deleted = false;

     this.cancelRide.Ride = this.activeRide;
     this.cancelRide.UserComment = this.userComment;
     this.cancelRide.UserComment.AppUserID = "-1";
     this.cancelRide.UserComment.RideID = "-1";
     if(this.activeRide.Status == "0")
     {
      this.service.CancelRideComplete(this.cancelRide).subscribe(
        data => {
          this.showComment = false;
          this.noActive = true;
          this.GetUserInfo();
          this.submitted = false;
        },
        error => {
          this.hasError = true;
          this.errorString = error.error.Message;
        }
      )
    }

    if(this.activeRide.Status == "6")
    {
      this.service.CommentRideComplete(this.cancelRide).subscribe(
        data => {
          this.showComment = false;
          this.noActive = true;
          this.hasActive = false;
          this.GetUserInfo();
        },
        error => {
          this.hasError = true;
          this.errorString = error.error.Message;
        }
      )
    }
  }

  ///Shows Form for Edit active ride, and sets form value to active ride values
  Edit(){
    this.showEdit = true;
    this.hasActive = false;
    this.EditForm.controls['streetName'].setValue(this.activeRide.StartLocation.Address.StreetName, {onlySelf: true});
    this.EditForm.controls['number'].setValue(this.activeRide.StartLocation.Address.Number, {onlySelf: true});
    this.EditForm.controls['town'].setValue(this.activeRide.StartLocation.Address.Town, {onlySelf: true});
    this.EditForm.controls['areaCode'].setValue(this.activeRide.StartLocation.Address.AreaCode, {onlySelf: true});
    this.EditForm.controls['carType'].setValue(this.activeRide.CarType, {onlySelf: true});
  }

  ///Cancels Editing active ride, and hides the form
  CancelEdit(){
    this.showEdit = false;
    this.hasActive = true;
    this.EditForm.reset();
  }

  ///Cancels Canceling and Commenting active ride, and hides the form
  CancelComment(){
    if(this.activeRide.Status != "6") {
      this.hasError = true;
      this.errorString = "You MUST leave a comment if you cancel your ride";
    }
    else {
      this.showComment = false;
      this.noActive = false;
      this.hasActive = true;
      this.CommentForm.reset();
    }
  }



}
