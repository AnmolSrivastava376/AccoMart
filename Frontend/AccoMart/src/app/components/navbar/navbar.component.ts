import { Component, OnInit } from '@angular/core';
import { jwtDecode } from "jwt-decode";



@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements  OnInit{
  isLoggedIn: boolean = false;
  username: string = '';

  constructor() { }


  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.isLoggedIn = true;
      const decoded = jwtDecode(token);
      console.log(decoded);
      console.log(2);
      }
  }

  logout(): void {
    
    localStorage.removeItem('token');
    this.isLoggedIn = false;
    this.username = '';
    console.log("Token removed successfully");
  }

}
