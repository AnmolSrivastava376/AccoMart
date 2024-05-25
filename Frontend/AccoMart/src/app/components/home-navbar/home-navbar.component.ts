import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { CommonModule } from '@angular/common';
import { SearchbarComponent } from '../searchbar/searchbar.component';
import { Product } from '../../interfaces/product';

@Component({
  selector: 'app-home-navbar',
  standalone: true,
  imports: [CommonModule,SearchbarComponent],
  templateUrl: './home-navbar.component.html',
  styleUrl: './home-navbar.component.css'
})
export class HomeNavbarComponent implements OnInit {
  isLoggedIn: boolean = false;
  username: string = '';
  prefix: string = '';
  @Output() searchCompleted: EventEmitter<Product[]> = new EventEmitter<Product[]>();
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
    this.router.navigate(['/home/auth'])
  }
  navigateToAuth(){
    this.router.navigate(['/home/auth'])
  }
  
  onSearchCompleted(products: Product[]): void {
    this.searchCompleted.emit(products);
  }
}
