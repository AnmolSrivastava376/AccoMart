import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CategoryService } from '../../services/category.services';
import { Category } from '../../interfaces/category';
import { FormsModule } from '@angular/forms'; // Import FormsModule
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { TokenHttpInterceptor } from '../../services/token-http-interceptor';


@Component({
  selector: 'app-admin-categories',
  standalone:true,
  imports: [CommonModule, FormsModule, HttpClientModule,NavbarComponent,SidebarComponent],
  providers:[CategoryService,TokenHttpInterceptor],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.css'

})
export class AdminCategoriesComponent implements OnInit {
  categories: Category[] = [];
  selectedCategory: Category = { categoryId: -1, categoryName: '' };
  isEditPopupOpen: boolean = false;
  isAddPopupOpen: boolean = false;
  categoryToAdd: string = '';
  isLoading:boolean =true;
  

  constructor(private router: Router, private categoryService: CategoryService) { }

  ngOnInit(): void {
    this.fetchCategories();
  }

  fetchCategories() {
    this.categoryService.fetchCategories().subscribe((response: any) => {
      this.categories = response;
      this.isLoading = false;
    }, error => {
      console.error('Error fetching categories:', error);
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

  
  editCategory(category: Category) {
    this.categoryService.editCategory(this.selectedCategory.categoryId,this.selectedCategory.categoryName).subscribe(response=>{
      this.isEditPopupOpen = false;
      this.fetchCategories();
      console.log("category edited successfully");
    },err=>{
      console.error("Error editing category:", err);

    });
  }

  saveCategory(categoryName: string) {
    this.categoryService.addCategory(categoryName).subscribe(response => {
      // After successfully adding the category, fetch the updated list of categories
      this.isAddPopupOpen = false;
      this.categoryToAdd = '';
      this.fetchCategories();
      console.log("Category added successfully");
    }, err => {
      console.error("Error adding category:", err);
    });
  }

  deleteCategory(Id: number) {
    if (confirm("Are you sure you want to delete this category?")) {
      this.categoryService.deleteCategory(Id).subscribe(response => {
        this.fetchCategories();
        console.log("deleted");
      });
    }
  }

  closeEditPopup() {
    this.isEditPopupOpen = false;
  }


  
  mergeResults(searchValue: string) {
    const searchNumber = parseInt(searchValue);
  
    if (!isNaN(searchNumber)) 
    { 
      this.categoryService.fetchCategorybyId(searchNumber).subscribe(response => {
        this.categories=[];
        if(response.categoryId)
          {
            this.categories.push(response);
          }
        this.isLoading = false;
      });
    } else {
        this.categoryService.fetchCategorybyName(searchValue).subscribe(response=>{
          this.categories=[];
          if(response.categoryId)
            {
              this.categories.push(response);
            }
        })
        this.isLoading =false;
       
    }
  }
  searchFunction(event: any) {

    this.isLoading = true;
    const searchValue = event.target.value;
    this.mergeResults(searchValue);
  }
  
}
