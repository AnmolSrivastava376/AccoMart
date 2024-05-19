// import { Component, OnInit } from '@angular/core';
// import { Router } from '@angular/router';
// import { NavbarComponent } from '../../components/navbar/navbar.component';
// import { SidebarComponent } from '../../components/sidebar/sidebar.component';
// import { CategoryService } from '../../services/category.services';
// import { Category } from '../../interfaces/category';
// import { FormsModule } from '@angular/forms'; // Import FormsModule

// import { CommonModule } from '@angular/common';
// import { MatIcon } from '@angular/material/icon';

// @Component({
//   selector: 'app-admin-categories',
//   standalone: true,
//   imports: [NavbarComponent, SidebarComponent,CommonModule,FormsModule,MatIcon],
//   templateUrl: './admin-categories.component.html',
//   styleUrl: './admin-categories.component.css'
// })
// export class AdminCategoriesComponent implements OnInit {
//   constructor(private router: Router,private categoryService:CategoryService) { }
//   categories: Category[] = [];

//     selectedCategory:Category = {
//       categoryId:-1,
//       categoryName:''
//    };

//    defaultCategory:Category = {
//     categoryId:-1,
//     categoryName:''
//  };

//   ngOnInit(): void {
//     this.fetchCategories();
//   }
//   isEditPopupOpen:boolean = false;
//   isAddPopupOpen:boolean =false;
//   categoryToAdd:string ='';

//   fetchCategories() {
//     this.categoryService.fetchCategories().then(response => {
//       this.categories = response.data;
//     }).catch(error => {
//       console.error('Error fetching categories:', error);
//     });
//   }

//   showProducts() {
//     this.router.navigate(['/admin/products']);
//   }

//   showCategories() {
//     this.router.navigate(['/admin/categories']);
//   }

//   deleteCategory(categoryId: number) {
//     // Implement delete functionality
//   }

//   editCategory(category: Category) {
//     this.selectedCategory = { ...category };
//     this.isEditPopupOpen = true;



//   editCategory() {
//     this.categoryService.editCategory(this.selectedCategory.categoryId,this.selectedCategory.categoryName).then(response=>{
//       this.isEditPopupOpen = false;
//       this.fetchCategories();
//     });

//   }

//   openEditPopup(newCategory: Category){
//     this.isEditPopupOpen = true;
//     this.selectedCategory = { ...category };
//     console.log(this.selectedCategory);
//   }

//   openAddPopup()
//   {

//     this.isAddPopupOpen = true;

//   }

//   closeAddPopup()
//   {
//     this.isAddPopupOpen = false;
//   }

//   saveCategory(categoryName: string) {
//     this.categoryService.addCategory(categoryName).then(response => {
//       // After successfully adding the category, fetch the updated list of categories
//       this.isAddPopupOpen = false;
//       this.categoryToAdd ='';
//       this.fetchCategories();
//       console.log("Category added successfully");
//     }).catch(err => {
//       console.error("Error adding category:", err);
//     });
//   }

//   deleteCategory(Id:number)
//   {
//     if (confirm("Are you sure you want to delete this category?")) {
//       this.categoryService.deleteCategory(Id).then(response=>{
//         this.fetchCategories();
//         console.log("deleted");
//       });
//     }
//   }

//   closeEditPopup()
//   {
//     this.isEditPopupOpen = false;
//   }



// }
