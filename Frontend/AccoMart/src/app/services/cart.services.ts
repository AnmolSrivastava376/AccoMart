import { Injectable } from '@angular/core';
import { CartStore } from '../store/cart-store';
import { cartItem } from '../interfaces/cartItem';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class cartService {
  constructor(private cartStore: CartStore) {}

  getCartItems$(): Observable<cartItem[]> {
    return this.cartStore.cartItems$;
  }
  setCartItems(items: cartItem[]): void {
    this.cartStore.cartItems = items;
    localStorage.setItem('cartItems', JSON.stringify(items));
  }
  fetchCart(): cartItem[] {
    return this.cartStore.cartItems;
  }

  addToCart(productId: number): void {
    if (!this.isPresentInCart(productId)) {
      const newItem: cartItem = { productId: productId, quantity: 1 };
      this.cartStore.cartItems = [...this.cartStore.cartItems, newItem];
    }
  }

  removeFromCart(productId: number): void {
    const updatedCart = this.cartStore.cartItems.filter(
      (item) => item.productId !== productId
    );
    this.cartStore.cartItems = updatedCart;
  }

  clearCart(): void {
    this.cartStore.cartItems = [];
  }

  incrementCountByProductId(productId: number): void {
    const updatedCart = this.cartStore.cartItems.map((item) => {
      if (item.productId === productId) {
        return { ...item, quantity: item.quantity + 1 };
      }
      return item;
    });
    this.cartStore.cartItems = updatedCart;
  }

  decrementCountByProductId(productId: number): void {
    const updatedCart = this.cartStore.cartItems
      .map((item) => {
        if (item.productId === productId && item.quantity > 0) {
          return { ...item, quantity: item.quantity - 1 };
        }
        return item;
      })
      .filter((item) => item.quantity !== 0);
    this.cartStore.cartItems = updatedCart;
  }

  findQuantityByProductId(productId: number): number {
    const item = this.cartStore.cartItems.find(
      (item) => item.productId === productId
    );
    return item ? item.quantity : 0;
  }

  isPresentInCart(productId: number): boolean {
    return this.cartStore.cartItems.some(
      (item) => item.productId === productId
    );
  }

  fetchQuantityInCart(): number {
    return this.cartStore.cartItems.length;
  }
}
