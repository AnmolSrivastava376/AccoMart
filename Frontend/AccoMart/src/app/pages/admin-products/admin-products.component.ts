import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import {  HttpClientModule } from '@angular/common/http';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { CategoryService } from '../../services/category.services';
import { Category } from '../../interfaces/category';
@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [NavbarComponent,CommonModule,SidebarComponent,HttpClientModule],
  providers : [productService,CategoryService],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.css'
})
export class AdminProductsComponent implements OnInit {
  constructor(private router: Router ,private productService: productService,private categoryService:CategoryService) { }

  products: Product[]=[{
    productId: 0,
    productName: '',
    productDesc: '',
    productPrice: 0,
    productImageUrl: '',
    categoryId: 0
  }];



  product:Product;
  categories:Category[];

  selectedProduct: Product;



  ngOnInit(): void {
    this.fetchCategories();

  }
  getCategoryName(categoryId: number): string {
    const category = this.categories.find(cat => cat.categoryId === categoryId);
    return category ? category.categoryName : '';
}



  fetchCategories(){
    this.categoryService.fetchCategories().subscribe(response=>{
     this.categories = response;
     this.fetchProducts();
    },err=>{
     console.log(err);
    })
   }

  async fetchProducts() {
    this.productService.fetchAllProducts().subscribe((response)=>{
      this.products = response;
    });
  }

  showProducts() {
    this.router.navigate(['/admin/products']);
  }

  showCategories() {
    this.router.navigate(['/admin/categories']);
  }

  openAddProductPage()
  {
    this.router.navigate([`/admin/product/add`]);
  }
  openEditPage(product: Product): void {
    this.selectedProduct = product;
    this.router.navigate([`/admin/product/edit/${product.productId}`]);
  }


  deleteProduct(productId: number) {
    this.productService.deleteProductById(productId).subscribe(
      () => {
        this.products = this.products.filter(product => product.productId !== productId);
      },
      error => {
        console.error('Error deleting product:', error);
      }
    );
  }

  openDeletePopup(product: Product)
  {
    // Ask the user for confirmation before deleting the product

    const confirmDelete = window.confirm(`Are you sure you want to delete ${product.productName}?`);

    // If the user confirms, proceed with deleting the product
    if (confirmDelete)
    {
      this.deleteProduct(product.productId);
    }
  }


}
