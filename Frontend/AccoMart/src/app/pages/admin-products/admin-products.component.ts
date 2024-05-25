import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import {  HttpClientModule } from '@angular/common/http';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { CategoryService } from '../../services/category.services';
import { Category } from '../../interfaces/category';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [NavbarComponent,CommonModule,SidebarComponent,HttpClientModule],
  providers : [productService,CategoryService],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.css'
})
export class AdminProductsComponent implements OnInit {
  constructor(private productService: productService,private categoryService:CategoryService) { }

  products: Product[];



  product:Product;
  categories:Category[];
  selectedProduct: Product;
  isLoading =true;


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
      this.isLoading = false;
    });
  }

  showProducts() {
    window.location.href = '/admin/products';
  }

  showCategories() {
    window.location.href = '/admin/categories';
  }

  openAddProductPage()
  {
    window.location.href = '/admin/product/add'
  }
  openEditPage(product: Product): void {
    this.selectedProduct = product;
    window.location.href = `/admin/product/edit/${product.productId}`;
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
    const confirmDelete = window.confirm(`Are you sure you want to delete ${product.productName}?`);
    if (confirmDelete)
    {
      this.deleteProduct(product.productId);
    }
  }

  mergeResults(searchValue: string) {
    const searchNumber = parseInt(searchValue);
  
    if (!isNaN(searchNumber)) 
    { 
      this.productService.fetchProductById(searchNumber).subscribe(response => {
        this.products=[];
        this.products.push(response);
        this.isLoading = false;
        console.log("here");
        console.log(this.products);
      });
    } else {
      forkJoin([
        this.productService.fetchProductByName(searchValue),
        this.productService.fetchProductByCategoryName(searchValue)
      ]).subscribe(([productsByName, productsByCategory]) => {
        this.products = [...productsByName, ...productsByCategory];
        this.isLoading = false;
      });
    }
  }
  

  searchFunction(event: any) {

    this.isLoading = true;
    const searchValue = event.target.value;
    this.mergeResults(searchValue);
  }

}
