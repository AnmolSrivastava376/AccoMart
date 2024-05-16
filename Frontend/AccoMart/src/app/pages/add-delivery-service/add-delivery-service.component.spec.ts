import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddDeliveryServiceComponent } from './add-delivery-service.component';

describe('AddDeliveryServiceComponent', () => {
  let component: AddDeliveryServiceComponent;
  let fixture: ComponentFixture<AddDeliveryServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddDeliveryServiceComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AddDeliveryServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
