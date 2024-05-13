import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForgetpasswordPageComponent } from './forgetpassword-page.component';

describe('ForgetpasswordPageComponent', () => {
  let component: ForgetpasswordPageComponent;
  let fixture: ComponentFixture<ForgetpasswordPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ForgetpasswordPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ForgetpasswordPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
