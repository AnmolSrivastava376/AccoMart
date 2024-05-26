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
import { HttpClientModule } from '@angular/common/http';
import { LoaderComponent } from '../../components/loader/loader.component';
import { FormsModule } from '@angular/forms';
import { HomeNavbarComponent } from '../../components/home-navbar/home-navbar.component';
import { SearchbarComponent } from '../../components/searchbar/searchbar.component';
import { SearchProductCardComponent } from '../../components/search-product-card/search-product-card.component';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [
    SearchProductCardComponent,
    HomeNavbarComponent,
    SearchbarComponent,
    CategoryNavbarComponent,
    ProductCardComponent,
    NavbarComponent,
    CommonModule,
    HttpClientModule,
    LoaderComponent,
    FormsModule,
  ],
  providers: [
    CategoryService,
    productService,
    Router,
    CartService,
    invoiceService,
  ],
})
export class HomeComponent implements OnInit, OnDestroy {
  categories: Category[] = [];
  products: Product[] = [];
  page = 1;
  minprice: number = 0;
  maxprice: number = Number.MAX_SAFE_INTEGER;
  filteredProducts: Product[] = [];
  searchActive: boolean = false;
  activeCategory: number = -1 || null;
  activeCategoryIndex: number = 0;
  cartItemLength = 0;
  isLoading: boolean = false;
  previousProduct: Product[];
  private cartSubscription: Subscription;

  constructor(
    private categoryService: CategoryService,
    private productService: productService,
    private cartService: CartService
  ) {}
  decodedToken: any;

  ngOnInit(): void {
    this.cartSubscription = this.cartService
      .getCartItems$()
      .subscribe((items) => {
        this.cartItemLength = items.length;
      });
    this.categoryService.fetchCategories().subscribe({
      next: (response) => {
        this.categories = response;
        this.activeCategory =
          this.categories.length > 0 ? this.categories[0].categoryId : 0;
        this.fetchProductsByCategory();
      },
      error: (error) => {
        console.error('Error fetching categories:', error);
      },
    });
  }

  fetchProductsByCategory(): void {
    if (this.activeCategory !== null) {
      this.isLoading = true;
      this.productService
        .fetchProductByPageNo(this.activeCategory, this.page)
        .subscribe({
          next: (response) => {
            this.products = response;
            this.isLoading = false;
          },
          error: (error) => {
            console.error('Error fetching products:', error);
            this.isLoading = false;
          },
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
    this.fetchProductsByCategory();
  }
  onIndexSelected(index: number) {
    this.activeCategoryIndex = index;
  }
  gotoCart() {
    window.location.href = '/home/cart';
  }
  handleNextPageLoad() {
    if (this.previousProduct?.length>0 || this.page===1) {
      this.page++;
      console.log('Fetching page no '+ this.page);
      this.previousProduct = []
      this.productService
        .fetchProductByPageNo(this.activeCategory, this.page)
        .subscribe((response) => {
          response.forEach((item) => {
            this.products = [...this.products, item];
            this.filteredProducts = [...this.filteredProducts, item];
            this.previousProduct = [...this.previousProduct, item];
          });
      });
    }
  }
  onFilteredProducts(filtered: Product[]) {
    this.filteredProducts = filtered;
  }

  onSearchCompleted(products: Product[]): void {
    this.products = products;
    this.searchActive = true;
  }
}
