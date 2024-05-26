import { Token } from '@angular/compiler';
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
      const decodedToken: any = jwtDecode(token);
      if (decodedToken.Role && decodedToken.Role === 'Admin') {
        return true;
      } else {
        return false;
      }
    }
    return false; 
  }

  userLoggedIn(){
    const token =localStorage.getItem('token');
    if(token){
      return true;
    }
    return false;
  }
}
