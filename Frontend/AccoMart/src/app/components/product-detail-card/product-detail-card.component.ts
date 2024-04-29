import { Component, Input } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-detail-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-detail-card.component.html',
  styleUrl: './product-detail-card.component.css'
})
export class ProductDetailCardComponent {
   @Input() product?:Product;
}
