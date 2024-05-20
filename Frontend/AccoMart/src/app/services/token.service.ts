
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  setToken(token: string): void {
    localStorage.setItem('token', token);
  }

  setRefreshToken(refreshToken:any):void{
    const refreshTokenString = JSON.stringify(refreshToken);
    localStorage.setItem('refreshtoken',refreshTokenString);
  }

  setAccessToken(accessToken:any):void
  {
    const accessTokenString = JSON.stringify(accessToken);
    localStorage.setItem('accesstoken',accessTokenString);

  }

  getAccessToken():any|null{
    const accessTokenString = localStorage.getItem('accesstoken');
    return accessTokenString;
  }

  getRefreshToken():any|null{
    const refreshTokenString = localStorage.getItem('accesstoken');
    return refreshTokenString;
  }
}