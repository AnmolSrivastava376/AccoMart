import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import axios from "axios";
import { Int32 } from 'mongodb';

@Injectable({
  providedIn: 'root',
})
export class HttpService {

  http = inject(HttpClient);
  constructor() {}

  login(email : string, password:string) {
    return this.http.post<{OTP:Number}>('http://localhost:5239/AuthenticationController/Login',{
      "email": email,
      "password": password
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
}
