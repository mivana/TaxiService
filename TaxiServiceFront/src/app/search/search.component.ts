import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/userService.service';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  SearchForm: FormGroup;
  submitted: boolean;

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
      created:[''],
      cancelled:[''],
      formed:[''],
      proccessed:[''],
      accepted:[''],
      failed:[''],
      successfull:['']
    })
  }

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
        
      }
    )
    var a = this.SearchForm.value;
  }

}
