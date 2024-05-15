import { Component, OnInit } from '@angular/core';
import { CategoryNavbarComponent } from '../../components/category-navbar/category-navbar.component';
import { Category } from '../../interfaces/category';
import { CategoryService } from '../../services/category.services';
import { ProductCardComponent } from '../../components/product-card/product-card.component';
import { Product } from '../../interfaces/product';
import { productService } from '../../services/product.services';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { cartItem } from '../../interfaces/cartItem';
import { InvoiceService } from '../../services/invoiceService';
import { MatCardLgImage } from '@angular/material/card';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [CategoryNavbarComponent, ProductCardComponent, NavbarComponent, CommonModule],
})
export class HomeComponent implements OnInit {
  categories: Category[] = [];
  products: Product[]=[{
    productId: 0,
    productName: '',
    productDesc: '',
    productPrice: 0,
    productImageUrl: '',
    categoryId: 0
  }];
  cart: cartItem[]=[]
  activeCategory: number=-1;
  activeCategoryIndex: number=0;
  constructor(private categoryService: CategoryService, private productService: productService,private router: Router,private invoiceService: InvoiceService) {}

  ngOnInit(): void {
    this.categoryService.fetchCategories()
      .then((response) => {
        this.categories = response.data;
        this.activeCategory=this.categories[0].categoryId;
      })
      .catch((error) => {
        console.error('Error fetching categories:', error);
      }).then(()=>{
        if(this.activeCategory!=-1){
          this.productService.fetchProductByCategoryID(this.activeCategory).then((response)=>{
            this.products = response.data;
          }).catch((error)=>{
            console.error('Error fetching products:', error);
          })
        }
      })

  }
  onCategorySelected(categoryId: number){
    this.activeCategory = categoryId;
    this.productService.fetchProductByCategoryID(this.activeCategory).then((response)=>{
      this.products = response.data;
    }).catch((error)=>{
      console.error('Error fetching categories:', error);
    })
  }
  onIndexSelected(index: number){
    this.activeCategoryIndex = index;
  }

  getInvoice(){
    console.log("Hello")
    this.invoiceService.getInvoice();
  }
  gotoCart(){
    this.router.navigate(['/home/cart']);
  }
}
