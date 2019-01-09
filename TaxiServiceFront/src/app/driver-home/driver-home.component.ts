import { Component, OnInit } from '@angular/core';
import { DriverAuthGuard } from '../guards/driver.guard';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { JwtInterceptor } from '../helper/jwt.interceptor';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../services/userService.service';
import { User } from '../models/User.model';
import { Ride } from '../models/Ride.model';
import { TakeRide } from '../models/TakeRide.model';
import { FinishRide } from '../models/FinishRide.model';

@Component({
  selector: 'app-driver-home',
  templateUrl: './driver-home.component.html',
  styleUrls: ['./driver-home.component.css'],
  providers:[
    DriverAuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
  ]
})
export class DriverHomeComponent implements OnInit {
  activeUser: User = new User();
  myRides: Ride[] = [];
  freeRides: Ride[] = [];
  activeRide: Ride = new Ride();
  takeRide: TakeRide = new TakeRide();
  fRide: FinishRide = new FinishRide();

  hasError: boolean;
  errorString: string = "";
  showFree: boolean;
  showHistory: boolean;
  myCarType: string;
  showFinish: boolean = false;
  showComment: boolean = false;
  hasActive: boolean = false;;
  
  CommentForm: FormGroup;
  submitted: boolean;
  result: boolean;

  constructor(private fb: FormBuilder,
              private service: UserService) { }

  ngOnInit() {
    this.CommentForm = this.fb.group({
      content: ['',Validators.required]
    });

    this.GetUserInfo();
    this.GetFreeRides();
  }

  ///When input changes, buttons are not disabled anymore
  ngAfterViewInit()
  {
    this.CommentForm.valueChanges.subscribe(
      data =>{
        this.submitted = false;
        this.result = false;
      }
    )
    
  }


  GetFreeRides(): any {
    this.service.GetFreeRides().subscribe(
      data=>{
        debugger
        this.freeRides = data;
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
        this.hasActive = false;
      }
    )
  }

   ///Gets Users Information
   GetUserInfo(){
    this.service.getUser().subscribe(
      data => {
        debugger
        var user = data;
        this.activeUser = user;
        this.myRides = user.DriverRides;
        this.myCarType = this.activeUser.DriverCars[0].CarType;
        this.activeRide = user.DriverRides[0];
        if(this.activeRide != null)
          this.hasActive = true;
        else
          this.hasActive = false;

      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
    )
  }

  Take(ride: Ride){
    this.service.TakeRide(ride.Id,ride).subscribe(
      data => {
        debugger
        this.activeRide = data;
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
    )
  }

  get c() { return this.CommentForm.controls; }

  OnSubmitComment(){
    this.submitted = true;
    this.hasError = false;
    this.errorString = "";

      
   // stop here if form is invalid
   if (this.CommentForm.invalid) {
    return;
   }

   this.fRide.FinishRide = this.activeRide;
   this.fRide.Content = this.CommentForm.controls['content'].value;
   this.fRide.IsGood = false;

   ///OVDE JE PROBLEM, NEKE ENTITETE DUPLIRA ILI TROPLIRA? :D POGLEDAJ OVO KASNIJE DETALJNIJE!

   this.service.FinishRide(this.fRide).subscribe(
     data=>{
       
     },
     error => {
       this.hasError = true;
       this.errorString = error.error.Message;
     }
   )
  }

  History(){
    this.showHistory = true;
    this.showFree = false;
  }

  Created(){
    this.showFree = true;
    this.showHistory = false;
  }

  Finish(){
    this.showFinish = true;
  }

  SuccessfullFinish(ride: Ride){

  }

  FailedFinish(ride: Ride){
    this.showComment = true;
  }

  CancelFinish(){
    this.showFinish = false;
    this.showComment = false;
  }


}
