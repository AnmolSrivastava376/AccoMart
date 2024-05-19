import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { CategoryService } from '../../services/category.services';
import { Category } from '../../interfaces/category';
import { FormsModule } from '@angular/forms'; // Import FormsModule

import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [NavbarComponent, SidebarComponent,CommonModule,FormsModule,MatIcon],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.css'
})
export class AdminCategoriesComponent implements OnInit {
  constructor(private router: Router,private categoryService:CategoryService) { }
  categories: Category[] = [];

    selectedCategory:Category = {
      categoryId:-1,
      categoryName:''
   };

   defaultCategory:Category = {
    categoryId:-1,
    categoryName:''
 };

  ngOnInit(): void {
    this.fetchCategories();
  }
  isEditPopupOpen:boolean = false;

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

  editCategory(category: Category) {
    this.selectedCategory = { ...category };
    this.isEditPopupOpen = true;

  }

  openEditPopup(category: Category){
    this.isEditPopupOpen = true;
    this.selectedCategory = { ...category };
    console.log(this.selectedCategory);
  }
  closeEditPopup()
  {
    this.isEditPopupOpen = false;
  }

  saveChanges()
  {

  }

}
