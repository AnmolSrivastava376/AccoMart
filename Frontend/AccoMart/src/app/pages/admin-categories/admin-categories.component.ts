import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { CategoryService } from '../../services/category.services';
import { Category } from '../../interfaces/category';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [NavbarComponent, SidebarComponent,CommonModule],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.css'
})
export class AdminCategoriesComponent implements OnInit {
  constructor(private router: Router,private categoryService:CategoryService) { }
  categories: Category[] = [];

  ngOnInit(): void {
    this.fetchCategories();
  }

  fetchCategories() {
    this.categoryService.fetchCategories().then(response => {
      this.categories = response.data;
    }).catch(error => {
      console.error('Error fetching categories:', error);
    });
  }

  showProducts() {
    this.router.navigate(['/admin/products']);
  }

  showCategories() {
    this.router.navigate(['/admin/categories']);
  }

  deleteCategory(categoryId: number) {
    // Implement delete functionality
  }

  editCategory(categoryId: number) {
    // Implement edit functionality
  }

}
