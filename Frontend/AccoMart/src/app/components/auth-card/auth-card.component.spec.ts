import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthCardComponent } from './auth-card.component';
import { HttpService } from '../../services/http.service';
import { TokenService } from '../../services/token.service';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { LoaderComponent } from '../loader/loader.component';
import { RouterTestingModule } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';

describe('AuthCardComponent', () => {
  let component: AuthCardComponent;
  let fixture: ComponentFixture<AuthCardComponent>;
  let httpService: HttpService;
  let tokenService: TokenService;
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        AuthCardComponent,
        LoaderComponent,
        ReactiveFormsModule,
        RouterTestingModule,
        HttpClientTestingModule
      ],
      declarations: [],
      providers: [HttpService, TokenService]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AuthCardComponent);
    component = fixture.componentInstance;
    httpService = TestBed.inject(HttpService);
    tokenService = TestBed.inject(TokenService);
    router = TestBed.inject(Router);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Register Form', () => {
    it('should invalidate the form when empty', () => {
      component.registerForm.controls['username'].setValue('');
      component.registerForm.controls['email'].setValue('');
      component.registerForm.controls['password'].setValue('');
      expect(component.registerForm.valid).toBeFalsy();
    });

    it('should validate the email field correctly', () => {
      const email = component.registerForm.controls['email'];
      email.setValue('invalidEmail');
      expect(email.valid).toBeFalsy();
      email.setValue('test@example.com');
      expect(email.valid).toBeTruthy();
    });

    it('should validate the password field correctly', () => {
      const password = component.registerForm.controls['password'];
      password.setValue('short');
      expect(password.valid).toBeFalsy();
      password.setValue('ValidPassword1!');
      expect(password.valid).toBeTruthy();
    });
  });

  describe('Login Form', () => {
    it('should invalidate the form when empty', () => {
      component.loginForm.controls['email'].setValue('');
      component.loginForm.controls['password'].setValue('');
      expect(component.loginForm.valid).toBeFalsy();
    });

    it('should validate the email field correctly', () => {
      const email = component.loginForm.controls['email'];
      email.setValue('invalidEmail');
      expect(email.valid).toBeFalsy();
      email.setValue('test@example.com');
      expect(email.valid).toBeTruthy();
    });
  });
});
