import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import axios from "axios";
import { Int32 } from 'mongodb';

@Injectable({
  providedIn: 'root',
})
export class HttpService {
  apiUrl = 'http://localhost:5280';
  http = inject(HttpClient);
  constructor() {}

  login(email : string, password:string) {
    return this.http.post<{OTP:Number}>('http://localhost:5239/AuthenticationController/Login',{
      "email": email,
      "password": password
    });
  }


  login2FA(email : string, OTP:Number) {
    return this.http.post<{token:string}>('http://localhost:5239/AuthenticationController/Login-2FA',{
      "email": email,
      "OTP": OTP
    });
  }
}
