import { Component, Input } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { cartItem } from '../../interfaces/cartItem';
import { CartStore } from '../../store/cart-store';
import { cartService } from '../../services/cart.services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-scroll-display-card',
  standalone: true,
  imports: [CommonModule, MatIcon],
  templateUrl: './product-scroll-display-card.component.html',
  styleUrl: './product-scroll-display-card.component.css',
})
export class ProductScrollDisplayCardComponent {
  @Input() products?: Product[];
  cart?: cartItem[];
  i = 0;
  constructor(private cartService: cartService, private router: Router) {
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
    this.router.navigate(['/home/productdetail/'+productId.toString()])
  }
}
