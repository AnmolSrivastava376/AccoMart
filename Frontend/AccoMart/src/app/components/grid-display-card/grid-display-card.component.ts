import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { MatIcon } from '@angular/material/icon';
import { Router } from '@angular/router';

@Component({
  selector: 'app-grid-display-card',
  standalone: true,
  imports: [CommonModule, HttpClientModule, MatIcon],
  templateUrl: './grid-display-card.component.html',
  styleUrl: './grid-display-card.component.css',
})
export class GridDisplayCardComponent {
  @Input() products?: Product[];
  constructor(private router : Router){}

  navigateToProductDetail(index: number) {
    const productId = this.products ? this.products[index].productId : 0;
     window.location.href = `home/productdetail/${productId}`
  }
}