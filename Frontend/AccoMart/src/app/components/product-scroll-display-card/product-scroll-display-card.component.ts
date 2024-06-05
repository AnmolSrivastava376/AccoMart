import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { cartItem } from '../../interfaces/cartItem';
import { CartService } from '../../services/cart.services';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-product-scroll-display-card',
  standalone: true,
  imports: [CommonModule, MatIcon],
  templateUrl: './product-scroll-display-card.component.html',
  styleUrl: './product-scroll-display-card.component.css',
})
export class ProductScrollDisplayCardComponent implements OnInit {
  cart?: cartItem[];
  i = 0;
  @Input() products?: Product[];
  @Input() categoryId: number;
  @Output() fetchNextPage: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output() fetchNextPageCategoryWise: EventEmitter<number> = new EventEmitter<number>();
  private cartSubscription: Subscription
  constructor(private cartService: CartService, private router : Router) {}

  ngOnInit(): void {
    this.cartSubscription = this.cartService.getCartItems$().subscribe({
      next:(item)=>{
        this.cart = item;
      }
    })
  }

  handleNextButtonClick() {
    if (this.products && this.i + 2 < this.products.length) {
      this.i++;
      if (this.i + 3 >= this.products.length) {
        this.fetchNextPage.emit(true);
        this.fetchNextPageCategoryWise.emit(this.categoryId);
      }
    }
  }

  handlePreviousButtonClick() {
    if (this.i > 0) {
      this.i--;
    }
  }

  addToCart(productId: number, stock: number): void {
    if (stock >= 1 && !this.isPresentInCart(productId)) {
      this.cartService.addToCart(productId);
    }
  }

  findQuantityByProductId(productId: number): number {
    return this.cartService.findQuantityByProductId(productId);
  }

  incrementCountByProductId(productId: number, stock: number): void {
    if (stock > this.cartService.findQuantityByProductId(productId)) {
      this.cartService.incrementCountByProductId(productId);
    } else {
      alert('Maximum stock limit reached for this item');
    }
  }

  isPresentInCart(productId: number): boolean {
    return this.cartService.isPresentInCart(productId);
  }

  decrementCountByProductId(productId: number): void {
    setTimeout(()=>{
      this.cartService.decrementCountByProductId(productId);
    }, 0)
  }

  removeElementByProductId(productId: number): void {
    this.cartService.removeFromCart(productId);
  }

  navigateToProduct(productId: number) {
    window.location.href = `home/productdetail/${productId}`;
  }
}
