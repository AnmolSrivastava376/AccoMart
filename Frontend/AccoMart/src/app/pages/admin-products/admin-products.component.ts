import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { HttpClient } from '@angular/common/http';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { DeleteProductPopupComponent } from '../../components/delete-product-popup/delete-product-popup.component';
import { EditProductPopupComponent } from '../../components/edit-product-popup/edit-product-popup.component';
@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [NavbarComponent,DeleteProductPopupComponent,EditProductPopupComponent,CommonModule],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.css'
})
export class AdminProductsComponent implements OnInit {
  constructor(private router: Router ,private http:HttpClient,private productService: productService) { }

  products: Product[]=[{
    productId: 0,
    productName: '',
    productDesc: '',
    productPrice: 0,
    productImageUrl: '',
    categoryId: 0
  }];

  product:Product;

  selectedProduct: Product;



  ngOnInit(): void {
    this.fetchProducts();
    
  }

  async fetchProducts() {
    this.productService.fetchAllProducts().then((response)=>{
      
      this.products = response.data;      
    }).catch((error)=>{
      console.error('Error fetching products:', error);
    })
    console.log(this.products.length+": length");

  }

  showProducts() {
    this.router.navigate(['/admin/products']);
  }

  showCategories() {
    this.router.navigate(['/admin/categories']);
  }


  openEditPage(product: Product): void {
    this.selectedProduct = product;
    this.router.navigate([`/admin/product/edit/${product.productId}`]);
  }

 
  openDeletePopup(product: Product): void {
    this.selectedProduct = product;
  }


}
