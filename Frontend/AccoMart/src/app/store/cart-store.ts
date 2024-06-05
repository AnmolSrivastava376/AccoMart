import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { cartItem } from '../interfaces/cartItem';
import { jwtDecode } from 'jwt-decode';
import axios from 'axios';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CartStore {
  baseUrl = environment.serverUrl;
  constructor() {
    const token = sessionStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
      this.cartId = this.decoded.CartId;
      axios
        .get(
          `${this.baseUrl}ShoppingCartController/Get/CartItems?cartId=${this.cartId}`
        )
        .then((response) => {
          this._cartItems = response.data;
          this._cartItemsSubject.next(this._cartItems);
          sessionStorage.setItem('cartItems', JSON.stringify(this._cartItems));
        });
    }
  }

  cartId: number;
  private _cartItems: cartItem[] = [];
  private _cartItemsSubject = new BehaviorSubject<cartItem[]>([]);
  decoded: { CartId: number; AddressId: number; UserId: string };
  cartItems$ = this._cartItemsSubject.asObservable();

  get cartItems(): cartItem[] {
    return this._cartItems;
  }

  set cartItems(items: cartItem[]) {
    this._cartItems = items;
    this._cartItemsSubject.next(items);
    this.updateSessionStorage();
    axios.post(
      `${this.baseUrl}ShoppingCartController/Add/CartItem?cartId=${this.cartId}`,
      items
    );
  }
  private updateSessionStorage(): void {
    sessionStorage.setItem('cartItems', JSON.stringify(this._cartItems));
  }
}