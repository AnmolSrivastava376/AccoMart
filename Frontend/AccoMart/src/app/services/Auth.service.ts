import { Injectable } from '@angular/core';
import { jwtDecode } from "jwt-decode";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor() { }

  isLoggedIn() {
    const token = localStorage.getItem('token');
    if (token) {
      // Decode the JWT token
      const decodedToken: any = jwtDecode(token);
      console.log(decodedToken);
      // Check if the decoded token contains the "admin" role
      if (decodedToken.Role && decodedToken.Role === 'Admin') {
        return true;
      } else {
        return false;
      }
    }
    return false; // Return false if no token found
  }
}
