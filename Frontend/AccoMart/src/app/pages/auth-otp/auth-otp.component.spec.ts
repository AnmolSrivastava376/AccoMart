import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthOtpComponent } from './auth-otp.component';

describe('AuthOtpComponent', () => {
  let component: AuthOtpComponent;
  let fixture: ComponentFixture<AuthOtpComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuthOtpComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AuthOtpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
