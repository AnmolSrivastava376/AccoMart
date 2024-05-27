import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  constructor() {}

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  setToken(token: string): void {
    localStorage.setItem('token', token);
  }

  setRefreshToken(refreshToken: any): void {
    localStorage.setItem('refreshtoken', refreshToken);
  }

  setAccessToken(accessToken: any): void {
    localStorage.setItem('accesstoken', accessToken);
  }

  setExpiryAccess(expiry_accesstoken: any): void {
    localStorage.setItem('expiry_accesstoken', expiry_accesstoken);
  }

  setExpiryRefresh(expiry_refreshtoken: any): void {
    localStorage.setItem('expiry_refreshtoken', expiry_refreshtoken);
  }

  getAccessToken(): any | null {
    const accessToken = localStorage.getItem('accesstoken');
    return accessToken;
  }

  getRefreshToken(): any | null {
    const refreshToken = localStorage.getItem('accesstoken');
    return refreshToken;
  }

  getAccessExpiry(): any | null {
    const expiry_accesstoken = localStorage.getItem('expiry_accesstoken');
    return expiry_accesstoken;
  }
  getRefreshExpiry(): any | null {
    const expiry_refreshtoken = localStorage.getItem('expiry_refreshtoken');
    return expiry_refreshtoken;
  }
}
