import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor() {}

  isAdminLoggedIn() {
    const token = sessionStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token);
      if (decodedToken.Role && decodedToken.Role === 'Admin') {
        return true;
      } else {
        return false;
      }
    }
    return false;
  }

  userLoggedIn() {
    const token = sessionStorage.getItem('token');
    if (token) {
      return true;
    }
    return false;
  }
}