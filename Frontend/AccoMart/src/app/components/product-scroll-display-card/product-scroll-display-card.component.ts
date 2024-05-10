import { Component, Input } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-scroll-display-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-scroll-display-card.component.html',
  styleUrl: './product-scroll-display-card.component.css'
})
export class ProductScrollDisplayCardComponent {
  @Input() products?:Product[]
}
