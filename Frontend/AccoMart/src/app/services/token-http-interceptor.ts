import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { TokenService } from './token.service';
import axios from 'axios';
import { RefreshToken } from '../interfaces/RefreshToken';


@Injectable()
export class TokenHttpInterceptor implements HttpInterceptor {
      constructor(private tokenService: TokenService, private router: Router) {}

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

    

    
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.tokenService.getToken();
    if (token) {
      let authReq: HttpRequest<any>;
      if (req.url.includes('api.cloudinary.com')) {
        // Clone the request without setting Authorization header
        authReq = req.clone();
      } else {
        // Clone the request with Authorization header
        authReq = req.clone({
          setHeaders: {
            'Authorization': `Bearer ${token}`
          }
        });
      }
      return next.handle(authReq).pipe(
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            // Redirect to /home/auth when unauthorize
              this.generateRefreshToken();
     
          }
          return throwError(error);
        })
      );
    }
    return next.handle(req);
  }

  async refreshToken(refresh: RefreshToken): Promise<void> {
    try {
      const response = await axios.post<{
        isSuccess: boolean;
        message: string;
        statusCode: number;
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
      
      }>('http://localhost:5239/AuthenticationController/Refresh-Token', refresh);
  

      this.tokenService.setToken(response.data.response.accessToken.token);
      this.tokenService.setAccessToken(response.data.response.accessToken.token);
      this.tokenService.setExpiryAccess(response.data.response.accessToken.expiryTokenDate);
      this.tokenService.setRefreshToken(response.data.response.refreshToken.token);
      this.tokenService.setExpiryRefresh(response.data.response.refreshToken.expiryTokenDate);

      console.log("done");
    } catch (error) {
      console.error('Error refreshing token:', error);
      // Handle error as needed
    }
  }


  generateRefreshToken(): void {
    const accessToken = this.tokenService.getAccessToken();
    const access_exp = this.tokenService.getAccessExpiry();
    const refresh_token = this.tokenService.getRefreshToken();
    const refresh_expiry = this.tokenService.getRefreshExpiry();

    this.ref.accessToken.token= accessToken;
    this.ref.accessToken.expiryTokenDate = access_exp;
    this.ref.refreshToken.token = refresh_token;
    this.ref.refreshToken.expiryTokenDate = refresh_expiry;

    this.refreshToken(this.ref);

  }


}
