import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [NavbarComponent,CommonModule,SidebarComponent,HttpClientModule],
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
    // Call your service method to delete the product by ID
    this.productService.deleteProductById(productId).then(() => {
      // After successful deletion, remove the product from the products array
      this.products = this.products.filter(product => product.productId !== productId);
    });
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
