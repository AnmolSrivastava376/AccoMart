import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  Router,
} from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './services/Auth.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class FrontendAuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router, private toastr: ToastrService) {}

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    if (!this.authService.userLoggedIn()) {
      this.router.navigate(['/unauthorized']);
      this.toastr.warning('Please Login');
      return false;
    }
    return true;
  }
}