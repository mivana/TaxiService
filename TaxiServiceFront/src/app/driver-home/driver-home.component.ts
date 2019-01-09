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
import { NumbericValidation } from '../validators/numeric-validator.validation';

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
  tempRides: Ride[] = [];
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
  hasActive: boolean = false;
  showSuccessfull: boolean = false;
;
  
  CommentForm: FormGroup;
  FinishForm: FormGroup;
  submitted: boolean;
  result: boolean;

  constructor(private fb: FormBuilder,
              private service: UserService) { }

  ngOnInit() {
    this.CommentForm = this.fb.group({
      content: ['',Validators.required]
    });

    this.FinishForm = this.fb.group({
      dStreetName:['',Validators.required],
      dNumber: ['',[Validators.required,NumbericValidation.numberValidator]],
      dTown: ['',Validators.required],
      dAreaCode: ['',[Validators.required,NumbericValidation.numberValidator]],
      price:['',[Validators.required,NumbericValidation.numberValidator]]
    })

    this.PrepareInfo();
    this.showHistory = true;
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

    this.FinishForm.valueChanges.subscribe(
      data =>{
        this.submitted = false;
        this.result = false;
      }
    )
  }

  PrepareInfo(){
    this.GetUserInfo();
    this.GetFreeRides();
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
        this.tempRides = user.DriverRides;
        this.myRides = this.tempRides.reverse();
        this.myCarType = this.activeUser.DriverCars[0].CarType;

        this.activeRide = this.myRides[0];
        if(this.activeRide != null && (this.activeRide.Status == "2" || this.activeRide.Status == "3" || this.activeRide.Status == "4")) 
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
        this.PrepareInfo();
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
    )
  }

  get c() { return this.CommentForm.controls; }
  get f() { return this.FinishForm.controls; }

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

   this.service.FinishRide(this.fRide).subscribe(
     data=>{
       this.showComment = false;
       this.hasActive = false;
       this.showFinish = false;
      this.PrepareInfo();
     },
     error => {
       this.hasError = true;
       this.errorString = error.error.Message;
     }
   )
  }

  OnSubmit(){
    this.submitted = true;
    this.hasError = false;
    this.errorString = "";

      
   // stop here if form is invalid
   if (this.FinishForm.invalid) {
    return;
   }

   this.fRide.FinishRide = this.activeRide;
   this.fRide.DStreetName = this.FinishForm.controls['dStreetName'].value;
   this.fRide.DNumber = this.FinishForm.controls['dNumber'].value;
   this.fRide.DTown = this.FinishForm.controls['dTown'].value;
   this.fRide.DAreaCode = this.FinishForm.controls['dAreaCode'].value;
   this.fRide.Price = this.FinishForm.controls['price'].value;
   this.fRide.IsGood = true;

   this.service.FinishRide(this.fRide).subscribe(
    data=>{
      this.showSuccessfull = false;
      this.hasActive = false;
      this.showFinish = false;
     this.PrepareInfo();
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
    this.GetUserInfo();
  }

  Created(){
    this.showFree = true;
    this.showHistory = false;
    this.GetFreeRides();
  }

  Finish(){
    this.showFinish = true;
  }

  SuccessfullFinish(ride: Ride){
    this.showSuccessfull = true;
  }

  FailedFinish(ride: Ride){
    this.showComment = true;
  }

  CancelFinish(){
    this.showFinish = false;
    this.showComment = false;
  }


}
