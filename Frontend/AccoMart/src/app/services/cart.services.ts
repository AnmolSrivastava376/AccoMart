import { Injectable } from '@angular/core';
import { cartItem } from '../interfaces/cartItem';
import { Observable } from 'rxjs';
import { cartItemService } from './cartItem.services';

@Injectable({
  providedIn: 'root',
})
export class cartService {
  constructor(private cartItemService : cartItemService) {}

  getCartItems$(): Observable<cartItem[]> {
    return this.cartItemService.cartItems$;
  }
  setCartItems(items: cartItem[]): void {
    this.cartItemService.cartItems = items;
    localStorage.setItem('cartItems', JSON.stringify(items));
  }
  fetchCart(): cartItem[] {
    return this.cartItemService.cartItems;
  }

  addToCart(productId: number): void {
    if (!this.isPresentInCart(productId)) {
      const newItem: cartItem = { productId: productId, quantity: 1 };
      this.cartItemService.cartItems = [...this.cartItemService.cartItems, newItem];
    }
  }

  removeFromCart(productId: number): void {
    const updatedCart = this.cartItemService.cartItems.filter(
      (item) => item.productId !== productId
    );
    this.cartItemService.cartItems = updatedCart;
  }

  clearCart(): void {
    this.cartItemService.cartItems = [];
  }

  incrementCountByProductId(productId: number): void {
    const updatedCart = this.cartItemService.cartItems.map((item) => {
      if (item.productId === productId) {
        return { ...item, quantity: item.quantity + 1 };
      }
      return item;
    });
    this.cartItemService.cartItems = updatedCart;
  }

  decrementCountByProductId(productId: number): void {
    const updatedCart = this.cartItemService.cartItems
      .map((item) => {
        if (item.productId === productId && item.quantity > 0) {
          return { ...item, quantity: item.quantity - 1 };
        }
        return item;
      })
      .filter((item) => item.quantity !== 0);
    this.cartItemService.cartItems = updatedCart;
  }

  findQuantityByProductId(productId: number): number {
    const item = this.cartItemService.cartItems.find(
      (item) => item.productId === productId
    );
    return item ? item.quantity : 0;
  }

  isPresentInCart(productId: number): boolean {
    return this.cartItemService.cartItems.some(
      (item) => item.productId === productId
    );
  }

  fetchQuantityInCart(): number {
    return this.cartItemService.cartItems.length;
  }
}
