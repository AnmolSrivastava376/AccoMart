import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { resetPassword } from '../interfaces/resetPassword';
import { Observable } from 'rxjs';
import { RefreshToken } from '../interfaces/RefreshToken';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class HttpService {
  constructor(private http: HttpClient) {}

  baseUrl = environment.serverUrl + 'AuthenticationController/';

  register(username: string, email: string, password: string) {
    return this.http.post<any>(
      `${this.baseUrl}Register`,
      {
        userName: username,
        email: email,
        password: password,
        roles: ['User'],
      }
    );
  }

  login(email: string, password: string) {
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
    }>(
      `${this.baseUrl}Login`,
      {
        email: email,
        password: password,
      }
    );
  }

  refreshToken(refreshToken: RefreshToken) {
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
    }>(
      `${this.baseUrl}Refresh-Token`,
      {
        refreshToken,
      }
    );
  }

  loginByEmail(email: string) {
    return this.http.post<{ OTP: Number }>(
      `${this.baseUrl}LoginByOtp?email=${email}`,
      {
        email: email,
      }
    );
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
    }>(
      `${this.baseUrl}Login-2FA?code=${OTP}&email=${email}`,
      {
        OTP: OTP,
        email: email,
      }
    );
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post<any>(
      `${this.baseUrl}forgot-password?email=${email}`,
      {}
    );
  }

  resetPassword(
    password: string,
    confirmPassword: string,
    token: string,
    email: string
  ): Observable<resetPassword> {
    return this.http.post<any>(
      `${this.baseUrl}reset-password`,
      {
        password: password,
        confirmPassword: confirmPassword,
        token: token,
        email: email,
      }
    );
  }
}
