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
import { cartService } from '../../services/cart.services';
import { Subscription } from 'rxjs';
import { invoiceService } from '../../services/invoiceService';
import { jwtDecode } from 'jwt-decode';
import { cartItemService } from '../../services/cartItem.services';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [CategoryNavbarComponent, ProductCardComponent, NavbarComponent, CommonModule,HttpClientModule],
  providers : [CategoryService, productService,Router,cartService,invoiceService,cartItemService]

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

  downloadFile(data: Blob): void {
    const blob = new Blob([data], { type: 'application/pdf' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'invoice.pdf';
    link.click();
    window.URL.revokeObjectURL(url);
  }
  activeCategory: number=-1 || null;
  activeCategoryIndex: number=0;
  cartItemLength = 0
  private cartSubscription: Subscription;
  constructor(private categoryService: CategoryService, private productService: productService,private router: Router, private cartService:cartService,private invoiceService : invoiceService, private cartItemService: cartItemService) {
  }
  decodedToken:any;
  ngOnInit(): void {
    const token = localStorage.getItem('token')
    if (token) {
      this.decodedToken = jwtDecode(token);
      console.log(this.decodedToken.CartId);
      this.cartItemService.fetchCartItemByCartId(this.decodedToken.CartId)
        .subscribe(
          (response) => {
            console.log(response, " : From Backend");
            this.cartService.setCartItems(response);
          },
          (error) => {
            console.error('Error fetching cart items:', error);
          }
        );
    }

    this.categoryService.fetchCategories()
      .subscribe(
        (response) => {
          this.categories = response;
          this.activeCategory = this.categories.length > 0 ? this.categories[0].categoryId : 0;
          this.fetchProductsByCategory();
        },
        (error) => {
          console.error('Error fetching categories:', error);
        }
      );
  }

  fetchProductsByCategory(): void {
    if (this.activeCategory !== null) {
      this.productService.fetchProductByCategoryID(this.activeCategory)
        .subscribe(
          (response) => {
            this.products = response;
          },
          (error) => {
            console.error('Error fetching products:', error);
          }
        );
    }
  }


  ngOnDestroy(): void {
    if (this.cartSubscription) {
      this.cartSubscription.unsubscribe();
    }
  }
  onCategorySelected(categoryId: number): void {
    this.activeCategory = categoryId;
    if (this.activeCategory !== null) {
      this.productService.fetchProductByCategoryID(this.activeCategory)
        .subscribe(
          (response) => {
            this.products = response;
          },
          (error) => {
            console.error('Error fetching products:', error);
          }
        );
    }
  }
  onIndexSelected(index: number){
    this.activeCategoryIndex = index;
  }
  gotoCart(){
    this.router.navigate(['/home/cart']);
  }

  getInvoice(): void {
    this.invoiceService.getInvoice().subscribe(
      (response: Blob) => {
        this.downloadFile(response);
      },
      (error) => {
        console.error('Error fetching invoice:', error);
      }
    );
  }
}
