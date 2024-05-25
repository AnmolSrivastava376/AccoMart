import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { GridDisplayCardComponent } from '../grid-display-card/grid-display-card.component';
import { ScrollDisplayCardComponent } from '../scroll-display-card/scroll-display-card.component';
import { ProductScrollDisplayCardComponent } from '../product-scroll-display-card/product-scroll-display-card.component';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [
    CommonModule,
    GridDisplayCardComponent,
    ScrollDisplayCardComponent,
    ProductScrollDisplayCardComponent,
    HttpClientModule,
  ],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.css',
})
export class ProductCardComponent {
  @Input() products?: Product[];
  @Input() categoryName?: string;
  @Output() fetchNextPage: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor() {}
  handleClick(productId: number) {
    window.location.href = `home/productdetail/${productId}`;
  }
  handleEmitter(){
    this.fetchNextPage.emit(true)
  }
}
