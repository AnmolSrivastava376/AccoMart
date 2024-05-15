import { Injectable } from '@angular/core';
import { CartStore } from '../store/cart-store';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private cartStore: CartStore) {}
  
  fetchCart() {
    return this.cartStore.cartItems;
  }

  addToCart(productId: number): void {
    if (!this.isPresentInCart(productId))
      this.cartStore.updateCart([
        ...this.cartStore.cartItems,
        { productId: productId, quantity: 1 },
      ]);
  }

  removeFromCart(productId: number): void {
    this.cartStore.updateCart(
      this.cartStore.cartItems.filter((item) => item.productId !== productId)
    );
  }

  clearCart(): void {
    this.cartStore.updateCart([]);
  }
  incrementCountByProductId(productId: number) {
    const updatedCart = this.cartStore.cartItems.map((item) => {
      if (item.productId === productId) {
        return { ...item, quantity: item.quantity + 1 };
      } else return item;
    });
    this.cartStore.updateCart(updatedCart);
  }
  decrementCountByProductId(productId: number) {
    const updatedCart = this.cartStore.cartItems
      .map((item) => {
        if (item.productId === productId && productId >= 0) {
          return { ...item, quantity: item.quantity - 1 };
        } else return item;
      })
      .filter((item) => item.quantity !== 0);
    this.cartStore.updateCart(updatedCart);
  }
  findQuantityByProductId(productId: number) {
    const item = this.cartStore.cartItems.find(
      (item) => item.productId === productId
    );
    return item ? item.quantity : 0;
  }
  isPresentInCart(productId: number) {
    return this.cartStore.cartItems.some(
      (item) => item.productId === productId
    );
  }
  fetchQuantityInCart(){
    return this.cartStore.cartItems.length
  }
}
