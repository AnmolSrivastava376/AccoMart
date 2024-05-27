import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChange,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Category } from '../../interfaces/category';
import { MatIconModule } from '@angular/material/icon';
import { HttpClientModule } from '@angular/common/http';
import { Product } from '../../interfaces/product';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-category-navbar',
  standalone: true,
  imports: [CommonModule, MatIconModule, HttpClientModule, FormsModule],
  templateUrl: './category-navbar.component.html',
  styleUrl: './category-navbar.component.css',
})
export class CategoryNavbarComponent implements OnChanges {
  @Input() categories?: Category[];
  @Input() products?: Product[];
  @Input() minprice: number;
  @Input() maxprice: number;
  @Output() categorySelected = new EventEmitter<number>();
  @Output() categoryIndex = new EventEmitter<number>();
  @Output() filteredProducts = new EventEmitter<Product[]>();
  isCategoryNavbarActive: boolean = false;
  isActiveAscendingFilter: boolean = false;
  isActiveDescendingFilter: boolean = false;
  isFirstLoad: boolean = true;
  shouldAnimate: boolean = false;
  isPriceFiltered: boolean = false;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['products']) {
      if (this.isActiveAscendingFilter) {
        this.sortPriceAscending();
      } else if (this.isActiveDescendingFilter) {
        this.sortPriceDscending();
      }
    }
    if (changes['products'] || changes['minprice'] || changes['maxprice']) {
      this.filterByPrice();
    }
  }

  onCategoryClick(categoryId: number, index: number) {
    this.categorySelected.emit(categoryId);
    this.categoryIndex.emit(index);
  }
  toogleCategoryButtonClick() {
    this.isCategoryNavbarActive = !this.isCategoryNavbarActive;
    if (this.isFirstLoad) {
      this.isFirstLoad = !this.isFirstLoad;
    }
    if (!this.isFirstLoad) {
      this.shouldAnimate = true;
    }
  }
  sortPriceAscending() {
    this.products = this.computeFilteredProducts();
    this.isPriceFiltered = false;
    this.isActiveAscendingFilter = true;
    this.isActiveDescendingFilter = false;
    this.filteredProducts.emit(this.products?.sort((a, b) => a.productPrice - b.productPrice));
  }

  sortPriceDscending() {
    this.products = this.computeFilteredProducts();
    this.isPriceFiltered = false;
    this.isActiveAscendingFilter = false;
    this.isActiveDescendingFilter = true;
    this.filteredProducts.emit(this.products?.sort((a, b) => b.productPrice - a.productPrice));
  }
  onMinPriceChange(event: any) {
    this.minprice = parseInt((event.target as HTMLInputElement).value, 10);
    this.filterByPrice();
  }
  onMaxPriceChange(event: any) {
    this.maxprice = parseInt((event.target as HTMLInputElement).value, 10);
    this.filterByPrice();
  }
  filterByPrice() {
    this.filteredProducts.emit(this.computeFilteredProducts())
  }
  computeFilteredProducts(): Product[]{
    this.isPriceFiltered = !this.isPriceFiltered;
    const minPrice = this.minprice || 0;
    const maxPrice = this.maxprice || Number.MAX_SAFE_INTEGER;
    const filteredProducts = this.products?.filter((product) => {
      return (
        product.productPrice >= minPrice && product.productPrice <= maxPrice
      );
    });
    return filteredProducts || [];
  }
}
