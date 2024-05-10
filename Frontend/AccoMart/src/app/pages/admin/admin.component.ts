// admin-dashboard.component.ts

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';


@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  showProducts() {
    this.router.navigate(['/admin/products']);
  }

  showCategories() {
    this.router.navigate(['/admin/categories']);
  }

}
