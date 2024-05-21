// edit-product-popup.component.ts
import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CreateProduct } from '../../interfaces/createProduct';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { productService } from '../../services/product.services';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { HttpClientModule } from '@angular/common/http';
import { Category } from '../../interfaces/category';
import { CategoryService } from '../../services/category.services';

@Component({
  selector: 'app-add-product-popup',
  templateUrl: './add-product.component.html',
  imports: [
    CommonModule,
    FormsModule,
    NavbarComponent,
    SidebarComponent,
    HttpClientModule,
  ],
  providers: [productService, CategoryService],
  standalone: true,
  styleUrls: ['./add-product.component.css'],
})
export class AddProductComponent implements OnInit {
  productImageUrl: string;
  uploading: boolean = false;
  categories: Category[];

  product: CreateProduct = {
    productName: '',
    productDesc: '',
    productPrice: 0,
    productImageUrl: '',
    categoryId: -1,
  };

  @Output() close = new EventEmitter<void>();
  constructor(
    private productService: productService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.fetchCategories();
  }

  fetchCategories() {
    this.categoryService.fetchCategories().subscribe(
      (response) => {
        this.categories = response;
        console.log(this.categories);
      },
      (err) => {
        console.log(err);
      }
    );
  }

  Cancel(): void {
    window.location.href = '/admin/products';
  }

  showProducts() {
    window.location.href = '/admin/products';
  }

  showCategories() {
    window.location.href = '/admin/categories';
  }

  AddProduct() {
    this.productService.addProduct(this.product).subscribe((response: any) => {
      console.log('Product added successfully:', response);
    });
  }
}
