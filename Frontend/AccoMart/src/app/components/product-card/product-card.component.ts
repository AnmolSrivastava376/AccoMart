import { Component, Input } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GridDisplayCardComponent } from '../grid-display-card/grid-display-card.component';
import { ScrollDisplayCardComponent } from '../scroll-display-card/scroll-display-card.component';
import { ProductScrollDisplayCardComponent } from '../product-scroll-display-card/product-scroll-display-card.component';
import { cartItem } from '../../interfaces/cartItem';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule,GridDisplayCardComponent,ScrollDisplayCardComponent, ProductScrollDisplayCardComponent],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.css'
})
export class ProductCardComponent{
  @Input() products?: Product[]
  @Input() categoryName?: string
  @Input() cart?: cartItem[]
  
  constructor(private router: Router) { }
  handleClick(productId: number) {
  this.router.navigate(['home/productdetail', productId.toString()]);
}
}
