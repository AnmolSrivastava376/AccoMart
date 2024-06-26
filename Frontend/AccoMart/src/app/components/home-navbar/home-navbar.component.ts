import {
  Component,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { CommonModule } from '@angular/common';
import { SearchbarComponent } from '../searchbar/searchbar.component';
import { Product } from '../../interfaces/product';
import { NavigationMenuComponent } from '../navigation-menu/navigation-menu.component';

@Component({
  selector: 'app-home-navbar',
  standalone: true,
  imports: [CommonModule, SearchbarComponent, NavigationMenuComponent, RouterLink],
  templateUrl: './home-navbar.component.html',
  styleUrl: './home-navbar.component.css',
})
export class HomeNavbarComponent implements OnInit {
  isLoggedIn: boolean = true;
  username: string = '';
  prefix: string = '';
  decoded: any;
  @Output() searchCompleted: EventEmitter<Product[]> = new EventEmitter<Product[]>();
  @ViewChild('screenMenu') screenMenu!: NavigationMenuComponent;
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
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('cartItems');
    sessionStorage.removeItem('accesstoken');
    sessionStorage.removeItem('refreshtoken');
    this.isLoggedIn = false;
    this.username = '';
    console.log('Token removed successfully');
    this.navigateToAuth();
  }

  navigateToAuth() {
    window.location.href = '/home/auth'
  }

  onSearchCompleted(products: Product[]): void {
    this.searchCompleted.emit(products);
  }

  toggleMenu() {
    this.screenMenu.toggleMenu();
  }
}