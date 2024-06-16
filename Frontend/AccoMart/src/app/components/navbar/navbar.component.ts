import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterLink],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent implements OnInit {
  isLoggedIn: boolean = false;
  username: string = '';
  decoded: any;
  constructor(private router: Router) {}

  ngOnInit(): void {
    const token = sessionStorage.getItem('token');
    if (token) {
      this.isLoggedIn = true;
      this.decoded = jwtDecode(token);
    }
    const userName = this.decoded?.UserName ? this.decoded.UserName : '';
    this.username = userName;
  }

  logout(): void {
    sessionStorage.clear()
    this.isLoggedIn = false;
    this.username = '';
    console.log('Token removed successfully');
    this.navigateToAuth();
  }

  navigateToHome()
  {
    this.router.navigate(['/home']);
  }
  
  navigateToAuth(){
    window.location.href = '/home/auth'
  }
}
