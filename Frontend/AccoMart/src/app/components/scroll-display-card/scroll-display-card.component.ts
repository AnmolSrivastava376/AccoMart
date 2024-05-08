import { Component, Input } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-scroll-display-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './scroll-display-card.component.html',
  styleUrl: './scroll-display-card.component.css'
})
export class ScrollDisplayCardComponent {
  @Input() products?:Product[];
}
