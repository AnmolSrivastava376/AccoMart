import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-grid-display-card',
  standalone: true,
  imports: [CommonModule,HttpClientModule,MatIcon],
  templateUrl: './grid-display-card.component.html',
  styleUrl: './grid-display-card.component.css'
})
export class GridDisplayCardComponent{
  @Input() products?: Product[]
  @Output() fetchNextPage: EventEmitter<boolean> = new EventEmitter<boolean>();
  i=0;
  navigateToProductDetail(index:number){
    const productId = this.products? this.products[index].productId : 0;
    window.location.href=`home/productdetail/${productId}`
  }

  handleNextButtonClick() {
    if (this.products && this.i + 2 < this.products.length) {
      this.i++;
      if(this.i+5>=this.products.length){
        this.fetchNextPage.emit(true);
      }
    }
  }
  handlePreviousButtonClick() {
    if (this.i > 0) {
      this.i--;
    }
  }
}
