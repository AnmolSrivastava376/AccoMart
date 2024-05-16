import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeliveryServicesComponent } from './delivery-services.component';

describe('DeliveryServicesComponent', () => {
  let component: DeliveryServicesComponent;
  let fixture: ComponentFixture<DeliveryServicesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeliveryServicesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DeliveryServicesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
