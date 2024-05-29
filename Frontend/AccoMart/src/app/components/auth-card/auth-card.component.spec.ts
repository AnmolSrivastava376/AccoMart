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

    // it('should display an error message when registration fails', () => {
    //   const errorMessage = 'Registration failed';
    //   spyOn(httpService, 'register').and.returnValue(throwError({ error: { message: errorMessage } }));
    
    //   // Set form values to valid values to pass form validation
    //   component.registerForm.controls['username'].setValue('test');
    //   component.registerForm.controls['email'].setValue('test@example.com');
    //   component.registerForm.controls['password'].setValue('ValidPassword1!');
    
    //   component.onRegister();
    
    //   // Expect that register method is called with correct parameters
    //   expect(httpService.register).toHaveBeenCalled();
    
    //   // Expect that errorMessage is set correctly
    //   expect(component.errorMessage).toEqual(errorMessage);
    // });
    
    

    // it('should navigate to login on successful registration', () => {
    //   const registerResponse = { status: 'Success', message: 'Registration successful' };
    //   spyOn(httpService, 'register').and.returnValue(of(registerResponse));
    
    //   spyOn(router, 'navigate');
    //   component.onRegister();
    
    //   expect(httpService.register).toHaveBeenCalled(); // Ensure register method is called
    //   expect(component.successMessage).toEqual(registerResponse.message);
    
    //   // Wait for async operations to complete
    //   fixture.detectChanges();
    //   fixture.whenStable().then(() => {
    //     expect(router.navigate).toHaveBeenCalledWith(['/home/auth']);
    //   });
    // });
    
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

    // it('should display an error message when login fails', () => {
    //   spyOn(httpService, 'login').and.returnValue(throwError({ error: { message: 'Login failed' } }));
    //   component.onLogin();
    //   fixture.detectChanges();
    //   expect(component.loginErrorMessage).toEqual('Login failed');
    // });

  //   it('should navigate to home on successful login', () => {
  //     const mockResponse = {
  //       isSuccess: true,
  //       response: {
  //         accessToken: { token: 'access-token', expiryTokenDate: 'expiry-date' },
  //         refreshToken: { token: 'refresh-token', expiryTokenDate: 'expiry-date' },
  //       },
  //       message: 'Login successful'
  //     };
  //     spyOn(httpService, 'login').and.returnValue(of(mockResponse));
  //     spyOn(tokenService, 'setToken');
  //     spyOn(tokenService, 'setAccessToken');
  //     spyOn(tokenService, 'setRefreshToken');
  //     spyOn(tokenService, 'setExpiryAccess');
  //     spyOn(tokenService, 'setExpiryRefresh');
  //     spyOn(router, 'navigate');
  //     component.onLogin();
  //     fixture.detectChanges();
  //     expect(tokenService.setToken).toHaveBeenCalledWith('access-token');
  //     expect(tokenService.setAccessToken).toHaveBeenCalledWith('access-token');
  //     expect(tokenService.setRefreshToken).toHaveBeenCalledWith('refresh-token');
  //     expect(tokenService.setExpiryAccess).toHaveBeenCalledWith('expiry-date');
  //     expect(tokenService.setExpiryRefresh).toHaveBeenCalledWith('expiry-date');
  //     expect(component.successMessage).toEqual('Login successful');
  //     expect(router.navigate).toHaveBeenCalledWith(['/home']);
  //   });
  });
});