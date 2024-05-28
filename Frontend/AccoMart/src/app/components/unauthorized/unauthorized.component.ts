import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  templateUrl: './unauthorized.component.html',
  styleUrls: ['./unauthorized.component.css'],
  providers: [Router]
})
export class UnauthorizedComponent {

  constructor(private router: Router) { }

  redirectToLogin(): void {
    this.router.navigate(['/home/auth']);
  }
}
