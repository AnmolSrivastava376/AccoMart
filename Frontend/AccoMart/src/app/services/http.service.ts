import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { resetPassword } from '../interfaces/resetPassword';
import { Observable } from 'rxjs';
import { RefreshToken } from '../interfaces/RefreshToken';

@Injectable({
  providedIn: 'root',
})
export class HttpService {

  http = inject(HttpClient);
  constructor() {}


  register(username: string, email: string, password: string) {
    return this.http.post<any>('http://localhost:5239/AuthenticationController', {
      userName: username,
      email: email,
      password: password,
      roles:["User"]
    });
  }

  login(email : string, password:string) {
    return this.http.post<{
      response: {
        accessToken: {
          token: string;
          expiryTokenDate: string;
        };
        refreshToken: {
          token: string;
          expiryTokenDate: string;
        };
      };
    }>('http://localhost:5239/AuthenticationController/Login',{
      "email": email,
      "password": password
    });
  }

  refreshToken(refreshToken : RefreshToken) {
    return this.http.post<{
      response: {
        accessToken: {
          token: string;
          expiryTokenDate: string;
        };
        refreshToken: {
          token: string;
          expiryTokenDate: string;
        };
      };
    }>('http://localhost:5239/AuthenticationController/Refresh-Token',{refreshToken});
  }


  loginForgotPwd(email : string) {
    return this.http.post<{OTP:Number}>(`http://localhost:5239/AuthenticationController/LoginForgotPassword?email=${email}`,{
      "email": email,
    });
  }

  login2FA(OTP: string, email: string) {
    return this.http.post<{
      response: {
        accessToken: {
          token: string;
          expiryTokenDate: string;
        };
        refreshToken: {
          token: string;
          expiryTokenDate: string;
        };
      };
    }>(`http://localhost:5239/AuthenticationController/Login-2FA?code=${OTP}&email=${email}`, {
      OTP: OTP,
      email: email
    });

  }
  forgotPassword(email: string): Observable<any> {
    return this.http.post<any>(`http://localhost:5239/AuthenticationController/forgot-password?email=${email}`, {});
  }

  resetPassword(resetPasswords : resetPassword): Observable<resetPassword> {
    return this.http.post<resetPassword>('http://localhost:5239/AuthenticationController/reset-password', resetPasswords);
  }
}
