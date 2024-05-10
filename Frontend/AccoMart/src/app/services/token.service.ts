
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
