import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { cartItemService } from '../../services/cartItem.services';
import { cartService } from '../../services/cart.services';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent implements OnInit {
  isLoggedIn: boolean = false;
  username: string = '';

  constructor(private router: Router) {}

  decoded: any;

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.isLoggedIn = true;
      this.decoded = jwtDecode(token);
    }
    const userName = this.decoded?.UserName ? this.decoded.UserName : '';
    this.username = userName;
  }
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('cartItems')
    localStorage.removeItem('accesstoken')
    localStorage.removeItem('refreshtoken')
    this.isLoggedIn = false;
    this.username = '';
    console.log('Token removed successfully');
    this.router.navigate(['home/auth'])
  }
  navigateToAuth(){
    this.router.navigate(['/home/auth']);
  }
}
