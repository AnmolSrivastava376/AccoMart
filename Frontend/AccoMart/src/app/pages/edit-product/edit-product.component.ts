// edit-product-popup.component.ts
import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Product } from '../../interfaces/product'; // adjust the path as needed
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ActivatedRoute, Router } from '@angular/router';
import { productService } from '../../services/product.services';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { HttpClientModule } from '@angular/common/http';
import { CategoryService } from '../../services/category.services';
import { Category } from '../../interfaces/category';
// import { AngularFireStorage } from '@angular/fire/storage';


@Component({
  selector: 'app-edit-product-popup',
  templateUrl: './edit-product.component.html',
  imports:[CommonModule,FormsModule,NavbarComponent,SidebarComponent,HttpClientModule],
  providers:[productService,CategoryService],
  standalone:true,
  styleUrls: ['./edit-product.component.css']
})

export class EditProductComponent implements OnInit {
  productImageUrl: string; // This will hold the URL of the uploaded image
  uploading: boolean = false;
  product: Product = {
    productId: 0,
    productName: '',
    productDesc: '',
    productPrice: 0,
    productImageUrl: '',
    categoryId: 0
  };
  categories:Category[];
  @Output() close = new EventEmitter<void>();
  constructor(private route: ActivatedRoute , private router:Router, private productService:productService,private categoryService:CategoryService) { }

  ngOnInit(): void {
    this.fetchProduct();
    this.fetchCategories();
  }

  fetchProduct()
  {
    this.route.params.subscribe(params => {
      const productId = +params['productId'];
      this.productService.fetchProductById(productId)
        .subscribe(
          response => {
            this.product = response;
            console.log(this.product);
          },
          error => {
            console.error('Error fetching product:', error);
          }
        );
    });
  }

  fetchCategories(){
    this.categoryService.fetchCategories().subscribe(response=>{
     this.categories = response;
     console.log(this.categories);
    },err=>{
     console.log(err);
    })
   }

  CancelEdit(): void {
    this.router.navigate(['/admin/products']);

  }

  showProducts() {
    this.router.navigate(['/admin/products']);
  }

  showCategories() {
    this.router.navigate(['/admin/categories']);
  }


  submitForm(): void {
    this.productService.editProductById(this.product.productId, this.product)
      .subscribe(
        (response: any) => {
          // Handle success, e.g., show a success message
          console.log('Product updated successfully:', response);
        },
        (error: any) => {
          // Handle error, e.g., show an error message
          console.error('Error updating product:', error);
        }
      );
  }


}
