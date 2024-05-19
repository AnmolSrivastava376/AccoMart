import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CategoryService } from '../../services/category.services';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { Category } from '../../interfaces/category';
import { FormsModule } from '@angular/forms'; // Import FormsModule
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';


@Component({
  selector: 'app-admin-categories',
  standalone:true,
  imports: [NavbarComponent, SidebarComponent,CommonModule,FormsModule,HttpClientModule],
  providers:[CategoryService],
  templateUrl: './admin-categories.component.html',
  styleUrls: ['./admin-categories.component.css']
})
export class AdminCategoriesComponent implements OnInit {
  categories: Category[] = [];
  selectedCategory: Category = { categoryId: -1, categoryName: '' };
  isEditPopupOpen: boolean = false;
  isAddPopupOpen: boolean = false;
  categoryToAdd: string = '';

  constructor(private router: Router, private categoryService: CategoryService) { }

  ngOnInit(): void {
    this.fetchCategories();
  }

  fetchCategories() {
    this.categoryService.fetchCategories().subscribe(response => {
      this.categories = response;
    }, error => {
      console.error('Error fetching categories:', error);
    });
  }

  showProducts() {
    this.router.navigate(['/admin/products']);
  }

  showCategories() {
    this.router.navigate(['/admin/categories']);
  }

  editCategory() {
    this.categoryService.editCategory(this.selectedCategory.categoryId, this.selectedCategory.categoryName).subscribe(response => {
      this.isEditPopupOpen = false;
      this.fetchCategories();
    }, error => {
      console.error('Error editing category:', error);
    });
  }

  openEditPopup(category: Category) {
    this.isEditPopupOpen = true;
    this.selectedCategory = { ...category };
    console.log(this.selectedCategory);
  }

  openAddPopup() {
    this.isAddPopupOpen = true;
  }

  closeAddPopup() {
    this.isAddPopupOpen = false;
  }

  saveCategory(categoryName: string) {
    this.categoryService.addCategory(categoryName).subscribe(response => {
      this.isAddPopupOpen = false;
      this.categoryToAdd = '';
      this.fetchCategories();
      console.log("Category added successfully");
    }, error => {
      console.error("Error adding category:", error);
    });
  }

  deleteCategory(categoryId: number) {
    if (confirm("Are you sure you want to delete this category?")) {
      this.categoryService.deleteCategory(categoryId).subscribe(response => {
        this.fetchCategories();
        console.log("Category deleted successfully");
      }, error => {
        console.error("Error deleting category:", error);
      });
    }
  }

  closeEditPopup() {
    this.isEditPopupOpen = false;
  }
}
