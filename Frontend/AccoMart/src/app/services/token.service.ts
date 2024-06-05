import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
  constructor() {}

  getToken(): string | null {
    return sessionStorage.getItem('token');
  }

  setToken(token: string): void {
    sessionStorage.setItem('token', token);
  }

  setRefreshToken(refreshToken: any): void {
    sessionStorage.setItem('refreshtoken', refreshToken);
  }

  setAccessToken(accessToken: any): void {
    sessionStorage.setItem('accesstoken', accessToken);
  }

  setExpiryAccess(expiry_accesstoken: any): void {
    sessionStorage.setItem('expiry_accesstoken', expiry_accesstoken);
  }

  setExpiryRefresh(expiry_refreshtoken: any): void {
    sessionStorage.setItem('expiry_refreshtoken', expiry_refreshtoken);
  }

  getAccessToken(): any | null {
    const accessToken = sessionStorage.getItem('accesstoken');
    return accessToken;
  }

  getRefreshToken(): any | null {
    const refreshToken = sessionStorage.getItem('accesstoken');
    return refreshToken;
  }

  getAccessExpiry(): any | null {
    const expiry_accesstoken = sessionStorage.getItem('expiry_accesstoken');
    return expiry_accesstoken;
  }
  getRefreshExpiry(): any | null {
    const expiry_refreshtoken = sessionStorage.getItem('expiry_refreshtoken');
    return expiry_refreshtoken;
  }
}