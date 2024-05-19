// edit-product-popup.component.ts
import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import {CreateProduct} from '../../interfaces/createProduct'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ActivatedRoute, Router } from '@angular/router';
import { productService } from '../../services/product.services';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';

// import { AngularFireStorage } from '@angular/fire/storage';


@Component({
  selector: 'app-add-product-popup',
  templateUrl: './add-product.component.html',
  imports:[CommonModule,FormsModule,NavbarComponent,SidebarComponent],
  standalone:true,
  styleUrls: ['./add-product.component.css']
})

export class AddProductComponent implements OnInit {
  productImageUrl: string; // This will hold the URL of the uploaded image
  uploading: boolean = false;

  product: CreateProduct = {
    productName: '',
    productDesc: '',
    productPrice: 0,
    productImageUrl: '',
    categoryId: 1
  };

  @Output() close = new EventEmitter<void>();
  constructor(private route: ActivatedRoute , private router:Router, private productService:productService) { }

  ngOnInit(): void {
  }

  Cancel(): void {
    this.router.navigate(['/admin/products']);

  }

  showProducts() {
    this.router.navigate(['/admin/products']);
  }

  showCategories() {
    this.router.navigate(['/admin/categories']);
  }

  AddProduct() {
    this.productService.addProduct(this.product)
      .subscribe(
        (response: any) => {
          // Handle success, e.g., show a success message
          console.log('Product added successfully:', response);
        },
        (error: any) => {
          // Handle error, e.g., show an error message
          console.error('Error adding product:', error);
        }
      );
  }


}
