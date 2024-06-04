import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpResponse,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { TokenService } from './token.service';
import axios from 'axios';
import { RefreshToken } from '../interfaces/RefreshToken';
import { environment } from '../../environments/environment';

@Injectable()
export class TokenHttpInterceptor implements HttpInterceptor {
  constructor(private tokenService: TokenService, private router: Router) {}

  baseUrl = environment.serverUrl;

  refToken: RefreshToken = {
    accessToken: {
      token: '',
      expiryTokenDate: '',
    },
    refreshToken: {
      token: '',
      expiryTokenDate: '',
    },
  };

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const token = this.tokenService.getToken();
    if (token) {
      let authReq: HttpRequest<any>;
      if (req.url.includes('api.cloudinary.com')) {
        authReq = req.clone();
      } else {
        authReq = req.clone({
          url: `${this.baseUrl}${req.url}`,
          setHeaders: {
            Authorization: `Bearer ${token}`,
          },
        });
      }
      return next.handle(authReq).pipe(
        tap((event) => {
          if (event instanceof HttpResponse) {
            // Log successful responses
            console.log('Successful Response:', event);
          }
        }),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            return this.handleUnauthorizedError();
          } else {
            return throwError(error);
          }
        })
      );
    }
    return next.handle(req);
  }

  private handleUnauthorizedError(): Observable<any> {
    // Attempt to refresh token
    return new Observable((observer) => {
      this.generateRefreshToken();
      observer.complete();
    });
  }

  private async refreshToken(refresh: RefreshToken): Promise<void> {
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
      }>(
        `AuthenticationController/Refresh-Token`,
        refresh
      );
      this.tokenService.setToken(response.data.response.accessToken.token);
      this.tokenService.setAccessToken(
        response.data.response.accessToken.token
      );
      this.tokenService.setExpiryAccess(
        response.data.response.accessToken.expiryTokenDate
      );
      this.tokenService.setRefreshToken(
        response.data.response.refreshToken.token
      );
      this.tokenService.setExpiryRefresh(
        response.data.response.refreshToken.expiryTokenDate
      );
    } catch (error) {
      console.error('Error refreshing token:', error);
    }
  }

  private generateRefreshToken(): void {
    const accessToken = this.tokenService.getAccessToken();
    const access_exp = this.tokenService.getAccessExpiry();
    const refresh_token = this.tokenService.getRefreshToken();
    const refresh_expiry = this.tokenService.getRefreshExpiry();
    this.refToken.accessToken.token = accessToken;
    this.refToken.accessToken.expiryTokenDate = access_exp;
    this.refToken.refreshToken.token = refresh_token;
    this.refToken.refreshToken.expiryTokenDate = refresh_expiry;
    this.refreshToken(this.refToken);
  }
}
