import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { TokenService } from './token.service';
import axios, { Axios } from 'axios';
import { RefreshToken } from '../interfaces/RefreshToken';


@Injectable()
export class TokenHttpInterceptor implements HttpInterceptor {
      constructor(private tokenService: TokenService, private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.tokenService.getToken();
    console.log("TokenHttpInterceptor", token);
    console.log(this.tokenService.getAccessToken());
    
    if (token) {
      const authReq = req.clone({
        setHeaders: {
          'Authorization': `Bearer ${token}`
        }
      });

      return next.handle(authReq).pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            // Redirect to /home/auth when unauthorized
            
            this.generateRefreshToken();
          }
          return throwError(error);
        })
      );
    }
    
    return next.handle(req);
  }


  refreshToken(refreshToken : RefreshToken) {
    return axios.post<{
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


  ref:RefreshToken={

    "accessToken": {
      "token": '',
      "expiryTokenDate": ''
    },
    "refreshToken": {
      "token": '',
      "expiryTokenDate": ''
  }
}
  generateRefreshToken(): void {
    const accessToken = this.tokenService.getAccessToken();
    const access_exp = this.tokenService.getAccessExpiry();
    const refresh_token = this.tokenService.getRefreshToken();
    const refresh_expiry = this.tokenService.getRefreshExpiry();

    this.ref.accessToken.token=accessToken;
    this.ref.accessToken.expiryTokenDate = access_exp;
    this.ref.refreshToken.token = refresh_token;
    this.ref.refreshToken.expiryTokenDate = refresh_expiry;

    this.refreshToken(this.ref);

  }


}
