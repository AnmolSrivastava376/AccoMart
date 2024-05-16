import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditDeliveryServiceComponent } from './edit-delivery-service.component';

describe('EditDeliveryServiceComponent', () => {
  let component: EditDeliveryServiceComponent;
  let fixture: ComponentFixture<EditDeliveryServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditDeliveryServiceComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EditDeliveryServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
