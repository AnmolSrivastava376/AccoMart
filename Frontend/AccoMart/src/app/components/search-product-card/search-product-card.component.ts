import { Component, Input } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { GridDisplayCardComponent } from '../grid-display-card/grid-display-card.component';
import { ScrollDisplayCardComponent } from '../scroll-display-card/scroll-display-card.component';
import { ProductScrollDisplayCardComponent } from '../product-scroll-display-card/product-scroll-display-card.component';
import { HttpClientModule } from '@angular/common/http';
import { SearchProductCardWithoutScrollComponent } from '../search-product-card-without-scroll/search-product-card-without-scroll.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-search-product-card',
  standalone: true,
  imports: [
    CommonModule,
    GridDisplayCardComponent,
    ScrollDisplayCardComponent,
    ProductScrollDisplayCardComponent,
    HttpClientModule,
    SearchProductCardWithoutScrollComponent,
  ],
  templateUrl: './search-product-card.component.html',
  styleUrl: './search-product-card.component.css',
})
export class SearchProductCardComponent {
  @Input() products?: Product[];
  @Input() categoryName?: string;
  constructor(private router : Router) {}

  handleClick(productId: number) {
    this.router.navigate(['home/productdetail', productId]);
  }
}