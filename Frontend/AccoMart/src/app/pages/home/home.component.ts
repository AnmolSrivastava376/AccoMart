import { Component, OnInit, OnDestroy } from '@angular/core';
import { CategoryNavbarComponent } from '../../components/category-navbar/category-navbar.component';
import { Category } from '../../interfaces/category';
import { CategoryService } from '../../services/category.services';
import { ProductCardComponent } from '../../components/product-card/product-card.component';
import { Product } from '../../interfaces/product';
import { productService } from '../../services/product.services';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CartService } from '../../services/cart.services';
import { Subscription } from 'rxjs';
import { invoiceService } from '../../services/invoiceService';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { LoaderComponent } from '../../components/loader/loader.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [
    CategoryNavbarComponent,
    ProductCardComponent,
    NavbarComponent,
    CommonModule,
    HttpClientModule,
    LoaderComponent,
    FormsModule
  ],
  providers: [
    CategoryService,
    productService,
    Router,
    CartService,
    invoiceService,
  ],
})
export class HomeComponent implements OnInit,OnDestroy {
  categories: Category[] = [];
  products: Product[] = [
    {
      productId: 0,
      productName: '',
      productDesc: '',
      productPrice: 0,
      productImageUrl: '',
      categoryId: 0,
      stock:0
    },
  ];
  minprice: number=0;
  maxprice: number=Infinity;
  filteredProducts: Product[] = [];

  downloadFile(data: Blob): void {
    const blob = new Blob([data], { type: 'application/pdf' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'invoice.pdf';
    link.click();
    window.URL.revokeObjectURL(url);
  }
  activeCategory: number = -1 || null;
  activeCategoryIndex: number = 0;
  cartItemLength = 0;
  isLoading : boolean =false;
  private cartSubscription: Subscription;

  constructor(
    private categoryService: CategoryService,
    private productService: productService,
    private router: Router,
    private cartService: CartService,
    private invoiceService: invoiceService,
  ) {}
  decodedToken: any;
  ngOnInit(): void {
    this.cartSubscription = this.cartService.getCartItems$().subscribe(
      items => {
        this.cartItemLength = items.length;
      }
    );
    this.categoryService.fetchCategories()
      .subscribe({
        next: (response) => {
          this.categories = response;
          this.activeCategory = this.categories.length > 0 ? this.categories[0].categoryId : 0;
          this.fetchProductsByCategory();
        },
        error: (error) => {
          console.error('Error fetching categories:', error);
        }
      });
  }

  fetchProductsByCategory(): void {
    if (this.activeCategory !== null) {
      this.isLoading = true;
      this.productService.fetchProductByCategoryID(this.activeCategory)
        .subscribe({
          next: (response) => {
            this.products = response;
            this.isLoading=false;
          },
          error: (error) => {
            console.error('Error fetching products:', error);
            this.isLoading=false;
          }
        });
    }
  }

  ngOnDestroy(): void {
    if (this.cartSubscription) {
      this.cartSubscription.unsubscribe();
    }
  }
  onCategorySelected(categoryId: number): void {
    this.activeCategory = categoryId;
    // if (this.activeCategory !== null) {
    //   this.productService.fetchProductByCategoryID(this.activeCategory)
    //     .subscribe({
    //       next: (response) => {
    //         this.products = response;
    //       },
    //       error: (error) => {
    //         console.error('Error fetching products:', error);
    //       }
    //     });
    // }
    this.fetchProductsByCategory();
  }
  onIndexSelected(index: number) {
    this.activeCategoryIndex = index;
  }
  gotoCart() {
    window.location.href = '/home/cart';
  }

  // getInvoice(): void {
  //   this.invoiceService.getInvoice(orderId).subscribe(
  //     (response: Blob) => {
  //       this.downloadFile(response);
  //     },
  //     (error) => {
  //       console.error('Error fetching invoice:', error);
  //     }
  //   );
  // }

  sortPriceAscending() {
    this.filteredProducts.sort((a, b) => a.productPrice - b.productPrice);
  }

  sortPriceDescending() {
    this.filteredProducts.sort((a, b) => b.productPrice - a.productPrice);
  }

  filterByPrice() {
    this.filteredProducts = this.products.filter(product =>
      product.productPrice >= this.minprice && product.productPrice <= this.maxprice
    );
  }
  onFilteredProducts(filtered: Product[]) {
    this.filteredProducts = filtered;
  }
}
