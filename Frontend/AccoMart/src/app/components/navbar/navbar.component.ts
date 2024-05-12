import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { jwtDecode } from "jwt-decode";



@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements  OnInit{
  isLoggedIn: boolean = false;
  username: string = '';

  constructor() { }

  decoded: { UserName: string };

ngOnInit(): void {
  const token = localStorage.getItem('token');

  if (token) {
    this.isLoggedIn = true;
    this.decoded = jwtDecode(token);
    console.log(this.decoded);
    console.log(2);
  }
  const userName = this.decoded?.UserName?this.decoded.UserName:'Anthony Gonsalves';
  this.username=userName
}
  logout(): void {

    localStorage.removeItem('token');
    this.isLoggedIn = false;
    this.username = '';
    console.log("Token removed successfully");
  }

}
