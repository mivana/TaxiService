import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderTaxiComponent } from './order-taxi.component';

describe('OrderTaxiComponent', () => {
  let component: OrderTaxiComponent;
  let fixture: ComponentFixture<OrderTaxiComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrderTaxiComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderTaxiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
