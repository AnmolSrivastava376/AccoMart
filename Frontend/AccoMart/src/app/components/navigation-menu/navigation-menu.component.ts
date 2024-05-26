import { AfterViewInit, Component, ElementRef, OnInit, Renderer2, ViewChild } from '@angular/core';
import { Route, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-navigation-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navigation-menu.component.html',
  styleUrl: './navigation-menu.component.css'
})
export class NavigationMenuComponent implements AfterViewInit, OnInit{
  isLoggedIn: boolean = true;
  username: string = '';
  @ViewChild('hamMenu', { static: true }) hamMenu!: ElementRef;
  @ViewChild('screenMenu', { static: true }) screenMenu!: ElementRef;
  decoded: any;
  constructor(private renderer: Renderer2, private router: Router) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.isLoggedIn = true;
      this.decoded = jwtDecode(token);
    }
    const userName = this.decoded?.UserName ? this.decoded.UserName : '';
    this.username = userName;
  }
  
  ngAfterViewInit() {
    this.renderer.listen(this.hamMenu.nativeElement, 'click', () => {
      this.toggleMenu();
    });
  }

  toggleMenu() {
    if (this.hamMenu.nativeElement.classList.contains('active')) {
      this.renderer.removeClass(this.hamMenu.nativeElement, 'active');
      this.renderer.setStyle(this.screenMenu.nativeElement, 'right', '-450px');
    } else {
      this.renderer.addClass(this.hamMenu.nativeElement, 'active');
      this.renderer.setStyle(this.screenMenu.nativeElement, 'right', '0');
    }
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('cartItems')
    localStorage.removeItem('accesstoken')
    localStorage.removeItem('refreshtoken')
    this.isLoggedIn = false;
    this.username = '';
    console.log('Token removed successfully');
    // this.router.navigate(['/home/auth'])
  }
}