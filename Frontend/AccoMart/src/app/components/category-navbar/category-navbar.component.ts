import { Component, EventEmitter, Input, OnChanges, Output, SimpleChange, SimpleChanges } from '@angular/core';
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
export class CategoryNavbarComponent implements OnChanges{
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

  ngOnChanges(changes: SimpleChanges): void {
    if(changes['products']){
      if (this.isActiveAscendingFilter) {
        console.log("sorted asc")
        this.sortPriceAscending();
      } else if (this.isActiveDescendingFilter) {
        console.log("sorted desc")
        this.sortPriceDscending();
      }
    }
  }

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
    this.isActiveAscendingFilter = !this.isActiveAscendingFilter;
    this.isActiveDescendingFilter = false;
    this.products?.sort((a, b) => a.productPrice - b.productPrice);
  }

  sortPriceDscending(){
    this.isActiveAscendingFilter = false;
    this.isActiveDescendingFilter = !this.isActiveDescendingFilter;
    this.products?.sort((a, b)=> b.productPrice - a.productPrice);
  }
  filterByPrice() {
    const filtered = this.products?.filter(product => 
      (product.productPrice >= (this.minprice || 0)) && (product.productPrice <= (this.maxprice || Infinity))
    ) ?? [];
    this.filteredProducts.emit(filtered);
  }
}
