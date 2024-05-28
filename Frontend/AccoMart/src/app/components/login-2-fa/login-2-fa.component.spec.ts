import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Login2FAComponent } from './login-2-fa.component';

describe('Login2FAComponent', () => {
  let component: Login2FAComponent;
  let fixture: ComponentFixture<Login2FAComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Login2FAComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(Login2FAComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});