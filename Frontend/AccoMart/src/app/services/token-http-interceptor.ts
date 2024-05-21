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


  refreshToken(refresh : RefreshToken) {
    return axios.post<{

        accessToken: {
          token: string;
          expiryTokenDate: string;
        };
        refreshToken: {
          token: string;
          expiryTokenDate: string;
        };

    }>('http://localhost:5239/AuthenticationController/Refresh-Token',refresh);
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

    this.ref.accessToken.token="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiS2h1c2hib28iLCJVc2VyTmFtZSI6IktodXNoYm9vIiwiVXNlcklkIjoiNTJlZTgxNTktNGZjNi00ZjhiLThhN2MtZThmNDE5NTI3YjAxIiwiQ2FydElkIjoiMzIiLCJqdGkiOiJjZTc4NTNkNi1mNDkwLTQ5NDYtOGRhOS05ZDMwNjgyZjRmYWUiLCJBZGRyZXNzSWQiOiI1IiwiUm9sZSI6IkFkbWluIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3MTYyNjA3MDQsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTIzOSIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTIzOSJ9.jWTeJVwd6wB9TS2Htut6nkUCGMIj23f63_yR4HIMJiA";
    this.ref.accessToken.expiryTokenDate = access_exp;
    this.ref.refreshToken.token = "Ik1zk5FupV4nTtu/TXsrIxqKC+480k0LpgHBTTSMdOgum9bL/QgGM9g8FqEWt3mSXO0O+hvL9VUnrK4hAWi7oA==";
    this.ref.refreshToken.expiryTokenDate = refresh_expiry;

    this.refreshToken(this.ref);

  }


}
