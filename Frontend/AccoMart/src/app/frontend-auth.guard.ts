import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
} from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './services/Auth.service';

@Injectable({
  providedIn: 'root',
})
export class FrontendAuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    if (!this.authService.userLoggedIn()) {
      this.router.navigate(['/unauthorized']);
      alert('Please Login');
      return false;
    }
    return true;
  }
}