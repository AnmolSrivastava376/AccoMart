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

  constructor() { }

  get cartItems(): cartItem[] {
    return this._cartItems;
  }
  updateCart(items: cartItem[]): void {
    this._cartItems = items;
    this._cartItemsSubject.next(items);
    this.updateLocalStorage();
  }

  private updateLocalStorage(): void {
    localStorage.setItem('cartItems', JSON.stringify(this._cartItems));
  }
}
