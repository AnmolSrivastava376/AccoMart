import { Component, Input, OnInit } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { cartItem } from '../../interfaces/cartItem';

import { CartService } from '../../services/cart.services';
import { Router } from '@angular/router';


@Component({
  selector: 'app-search-product-card-without-scroll',
  standalone: true,
  imports: [CommonModule, MatIcon],
  templateUrl: './search-product-card-without-scroll.component.html',
  styleUrl: './search-product-card-without-scroll.component.css'
})
export class SearchProductCardWithoutScrollComponent {
  @Input() products?: Product[];
  cart?: cartItem[];
  i = 0;
  constructor(private cartService: CartService, private router: Router) {
    this.cart = this.cartService.fetchCart();
  }

  handleNextButtonClick() {
    if (this.products && this.i + 2 < this.products.length) {
      this.i++;
    }
  }
  handlePreviousButtonClick() {
    if (this.i > 0) {
      this.i--;
    }
  }
  addToCart(productId: number): void{
    this.cartService.addToCart(productId);
  }
  findQuantityByProductId(productId: number): number{
    return this.cartService.findQuantityByProductId(productId);
  }
  incrementCountByProductId(productId: number): void{
    this.cartService.incrementCountByProductId(productId);
  }
  isPresentInCart(productId: number): boolean {
    return this.cartService.isPresentInCart(productId);
  }
  decrementCountByProductId(productId: number): void{
    this.cartService.decrementCountByProductId(productId);
  }
  removeElementByProductId(productId: number): void{
    this.cartService.removeFromCart(productId);
  }
  navigateToProduct(productId:number){
    this.router.navigate(['/home/productdetail/',productId])
  }


}
