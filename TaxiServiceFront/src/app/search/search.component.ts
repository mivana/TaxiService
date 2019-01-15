import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/userService.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Ride } from '../models/Ride.model';
import { SessionService } from '../services/sessionservice.service';
import { element } from '@angular/core/src/render3';
import { SortModel } from '../models/Sort.Model';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  SearchForm: FormGroup;
  StatusForm: FormGroup;
  SearchAdminForm: FormGroup;

  submitted: boolean;
  searched: boolean = false;
  showError: boolean = false;

  resultRides: Ride[]= null;
  copyRides: Ride[]= null;
  tempRides: Ride[]= null;
  sortBy: SortModel = new SortModel();
  hasError: boolean;
  errorString: string = "";
  hasErrorSearch: boolean = false;
  

  constructor(private fb: FormBuilder,
              private service: UserService) { }

  ngOnInit() {
    this.SearchForm = this.fb.group({
      dateFrom:[''],
      dateTo: [''],
      ratingFrom:[''],
      ratingTo:[''],
      priceFrom:[''],
      priceTo:[''],
      searchMine:['false'],
      searchAll:[''],
      searchMineD:['false'],
      searchFree:['']
    })

    this.StatusForm = this.fb.group({
      created:['false'],
      cancelled:['false'],
      formed:['false'],
      proccessed:['false'],
      accepted:['false'],
      failed:['false'],
      successfull:['false']
    });

    this.SearchAdminForm = this.fb.group({
      dName:[''],
      dSurname:[''],
      cName:[''],
      cSurname:[''],
    })


    if(this.isAdmin())
      this.SearchForm.controls['searchAll'].setValue(true);
  }

  ngAfterViewInit()
  {
    this.SearchForm.valueChanges.subscribe(
      data =>{
        this.submitted = false;
      }
    )

    this.SearchForm.controls['searchMine'].valueChanges.subscribe(
      data=>{
        this.submitted = false;
        if(data == true)
          this.SearchForm.controls['searchAll'].setValue(false);
      }
    )

    this.SearchForm.controls['searchAll'].valueChanges.subscribe(
      data=>{
        this.submitted = false;
        if(data == true)
          this.SearchForm.controls['searchMine'].setValue(false);
      }
    )

    this.SearchForm.controls['searchMineD'].valueChanges.subscribe(
      data=>{
        this.submitted = false;
        if(data == true)
          this.SearchForm.controls['searchFree'].setValue(false);
      }
    )

    this.SearchForm.controls['searchFree'].valueChanges.subscribe(
      data=>{
        this.submitted = false;
        if(data == true)
          this.SearchForm.controls['searchMineD'].setValue(false);
      }
    )

    this.StatusForm.valueChanges.subscribe(
      data => {
        this.hasErrorSearch = false;
        this.submitted = false;
        this.showError = false
        if(!this.searched)
        {
          this.showError = true;
          return;
        }

        this.resultRides = this.copyRides;
        var filter = "";
        if(this.StatusForm.controls['created'].value == true)
          filter += "0";
        if(this.StatusForm.controls['cancelled'].value == true)
          filter += ",1";
        if(this.StatusForm.controls['formed'].value == true)
          filter += ",2";
        if(this.StatusForm.controls['proccessed'].value == true)
          filter += ",3";
        if(this.StatusForm.controls['accepted'].value == true)
          filter += ",4";
        if(this.StatusForm.controls['failed'].value == true)
          filter += ",5";
        if(this.StatusForm.controls['successfull'].value == true)
          filter += ",6";

        if(filter != "")
          this.FilterSearch(filter);
      }
    )

    this.SearchAdminForm.valueChanges.subscribe(
      data =>{
        this.submitted = false;
      }
    )
  }

  //POGLEDAJ da stavis deafult value da na false, da nije kada stigne na back da je null! mozda ga to zeza <3
  get f() { return this.SearchForm.controls; }

  OnSubmit(){
    this.submitted = true;
    debugger

     // stop here if form is invalid
     if (this.SearchForm.invalid) {
      return;
    } 

    this.service.Search(this.SearchForm.value).subscribe(
      data => {
        var temp = data;
        if(temp == [])
          this.resultRides = null
        else
          this.resultRides = temp;
        this.copyRides = this.resultRides;
        this.searched = true;
        this.showError = false
        this.hasErrorSearch = false;
        this.submitted = false;
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
    )
  }

  OnSubmitAdmin(){
    this.submitted = true;
    debugger

     // stop here if form is invalid
     if (this.SearchAdminForm.invalid) {
      return;
    } 

    this.service.SearchAdmin(this.SearchAdminForm.value).subscribe(
      data => {
        var temp = data;
        if(temp == [])
          this.resultRides = null
        else
          this.resultRides = temp;
        this.copyRides = this.resultRides;
        this.searched = true;
        this.showError = false
        this.hasErrorSearch = false;
        this.submitted = false;
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
      }
    )
  }

  FilterSearch(filter: string){
    debugger
    var tokens = filter.split(',');
    this.tempRides = this.resultRides;
    this.resultRides = [];
    this.hasErrorSearch = false;
    
    this.tempRides.forEach(element =>{
      tokens.forEach(token =>{
        if(token == element.Status)
          this.resultRides.push(element);
      });
      
    });

  }

  SortDate(){
    this.submitted = true;
    if(this.searched == false)
    {
      this.hasErrorSearch = true;
      return
    }

    this.sortBy.SortBy = "Date";
    this.sortBy.ResultList = this.resultRides;
    this.service.Sort(this.sortBy).subscribe(
      data => {
        this.tempRides = data;
        this.resultRides = this.tempRides.reverse();
        this.copyRides = this.resultRides;
        this.submitted = false;
        this.hasErrorSearch = false;
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
        this.submitted = false;
      }
    )
  }

  SortRating(){
    this.submitted = true;
    if(this.searched == false)
    {
      this.hasErrorSearch = true;
      return
    }

    this.sortBy.SortBy = "Rating";
    this.sortBy.ResultList = this.resultRides;
    this.service.Sort(this.sortBy).subscribe(
      data => {
        this.tempRides = data;
        this.resultRides = this.tempRides.reverse();
        this.copyRides = this.resultRides;
        this.submitted = false;
        this.hasErrorSearch = false;
      },
      error => {
        this.hasError = true;
        this.errorString = error.error.Message;
        this.submitted = false;
      }
    )
  }

  NotUser(){
    if(SessionService.isUser())
      return false;
    else
      return true;
  }

  isAdmin(){
    return SessionService.isAdmin();
  }

  isDriver(){
    return SessionService.isDriver();
  }

}
