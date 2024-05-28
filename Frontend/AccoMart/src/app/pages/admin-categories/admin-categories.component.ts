import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CategoryService } from '../../services/category.services';
import { Category } from '../../interfaces/category';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { TokenHttpInterceptor } from '../../services/token-http-interceptor';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    NavbarComponent,
    SidebarComponent,
  ],
  providers: [CategoryService, TokenHttpInterceptor, ToastrService],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.css',
})
export class AdminCategoriesComponent implements OnInit {
  categories: Category[] = [];
  selectedCategory: Category = { categoryId: -1, categoryName: '' };
  isEditPopupOpen: boolean = false;
  isAddPopupOpen: boolean = false;
  categoryToAdd: string = '';
  isLoading: boolean = true;
  constructor(
    private categoryService: CategoryService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.fetchCategories();
  }

  fetchCategories() {
    this.categoryService.fetchCategories().subscribe(
      (response: any) => {
        this.categories = response;
        this.isLoading = false;
      },
      (error) => {
        this.toastr.error('Error  Fetching products', undefined, {
          timeOut: 5000,
        });
      }
    );
  }

  openEditPopup(category: Category) {
    this.isEditPopupOpen = true;
    this.selectedCategory = { ...category };
  }

  openAddPopup() {
    this.isAddPopupOpen = true;
  }

  closeAddPopup() {
    this.isAddPopupOpen = false;
  }

  editCategory(category: Category) {
    this.categoryService
      .editCategory(
        this.selectedCategory.categoryId,
        this.selectedCategory.categoryName
      )
      .subscribe(
        (response) => {
          this.isEditPopupOpen = false;
          this.fetchCategories();
          this.toastr.success('Category edited', undefined, { timeOut: 5000 });
        },
        (err) => {
          this.toastr.error('Error editing category', undefined, {
            timeOut: 5000,
          });
        }
      );
  }

  saveCategory(categoryName: string) {
    this.categoryService.addCategory(categoryName).subscribe(
      (response) => {
        this.isAddPopupOpen = false;
        this.categoryToAdd = '';
        this.fetchCategories();
        this.toastr.success('Category added successfully', undefined, {
          timeOut: 5000,
        });
      },
      (err) => {
        this.toastr.error('Error creating category', undefined, {
          timeOut: 5000,
        });
      }
    );
  }

  deleteCategory(Id: number) {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.deleteCategory(Id).subscribe((response) => {
        this.fetchCategories();
        this.toastr.success('Category deleted successfully', undefined, {
          timeOut: 5000,
        });
      });
    }
  }

  closeEditPopup() {
    this.isEditPopupOpen = false;
  }

  mergeResults(searchValue: string) {
    const searchNumber = parseInt(searchValue);
    if (!isNaN(searchNumber)) {
      this.categoryService
        .fetchCategorybyId(searchNumber)
        .subscribe((response) => {
          this.categories = [];
          if (response.categoryId) {
            this.categories.push(response);
          }
          this.isLoading = false;
        });
    } else {
      this.categoryService
        .fetchCategorybyName(searchValue)
        .subscribe((response) => {
          this.categories = [];
          if (response.categoryId) {
            this.categories.push(response);
          }
        });
      this.isLoading = false;
    }
  }

  searchFunction(event: any) {
    this.isLoading = true;
    const searchValue = event.target.value;
    this.mergeResults(searchValue);
  }
}