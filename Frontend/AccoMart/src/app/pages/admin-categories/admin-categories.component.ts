import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.css'
})
export class AdminCategoriesComponent implements OnInit {
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
