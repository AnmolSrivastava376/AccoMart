import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Category } from '../../interfaces/category';
import {MatIconModule} from '@angular/material/icon';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import { Product } from '../../interfaces/product';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-category-navbar',
  standalone: true,
  imports: [CommonModule, MatIconModule,HttpClientModule, FormsModule],
  templateUrl: './category-navbar.component.html',
  styleUrl: './category-navbar.component.css'
})
export class CategoryNavbarComponent {
  @Input() categories?: Category[];
  @Input() products?: Product[];
  @Input() minprice: number=0;
  @Input() maxprice: number=Number.MAX_SAFE_INTEGER;
  @Output() categorySelected = new EventEmitter<number>();
  @Output() categoryIndex = new EventEmitter<number>();
  @Output() filteredProducts = new EventEmitter<Product[]>();
  isCategoryNavbarActive: boolean = false;
  isActiveAscendingFilter: boolean = false;
  isActiveDescendingFilter: boolean = false;
  isFirstLoad: boolean = true;
  shouldAnimate:boolean = false;

  onCategoryClick(categoryId: number, index: number) {
    this.categorySelected.emit(categoryId);
    this.categoryIndex.emit(index);
  }
  toogleCategoryButtonClick(){
    this.isCategoryNavbarActive=!this.isCategoryNavbarActive;
    if(this.isFirstLoad){
      this.isFirstLoad = !this.isFirstLoad;
    }
    if(!this.isFirstLoad){
      this.shouldAnimate=true;
    }
  }
  sortPriceAscending(){
    this.isActiveAscendingFilter = true;
    this.isActiveDescendingFilter = false;
    this.products?.sort((a, b) => a.productPrice - b.productPrice);
  }

  sortPriceDscending(){
    this.isActiveAscendingFilter = false;
    this.isActiveDescendingFilter = true;
    this.products?.sort((a, b)=> b.productPrice - a.productPrice);
  }
  filterByPrice() {
    const filtered = this.products?.filter(product => 
      (product.productPrice >= (this.minprice || 0)) && (product.productPrice <= (this.maxprice || Infinity))
    ) ?? [];
    this.filteredProducts.emit(filtered);
  }
}
