import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { cartItem } from '../interfaces/cartItem';

@Injectable({
  providedIn: 'root'
})
export class CartStore {
  private _cartItems: cartItem[] = [];
  private _cartItemsSubject = new BehaviorSubject<cartItem[]>([]);

  cartItems$ = this._cartItemsSubject.asObservable();

  get cartItems(): cartItem[] {
    return this._cartItems;
  }

  set cartItems(items: cartItem[]) {
    this._cartItems = items;
    this._cartItemsSubject.next(items);
    this.updateLocalStorage();
  }

  constructor() {
    const storedCartItems = localStorage.getItem('cartItems');
    if (storedCartItems) {
      this._cartItems = JSON.parse(storedCartItems);
      this._cartItemsSubject.next(this._cartItems);
    }
  }

  private updateLocalStorage(): void {
    localStorage.setItem('cartItems', JSON.stringify(this._cartItems));
  }
}
