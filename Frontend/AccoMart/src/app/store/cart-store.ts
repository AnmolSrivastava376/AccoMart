import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { cartItem } from '../interfaces/cartItem';
import { cartItemService } from '../services/cartItem.services';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class CartStore {

  constructor(private cartItemService : cartItemService) {
    const storedCartItems = localStorage.getItem('cartItems');
    if (storedCartItems) {
      this._cartItems = JSON.parse(storedCartItems);
      this._cartItemsSubject.next(this._cartItems);
    }

  }
  private _cartItems: cartItem[] = [];
  private _cartItemsSubject = new BehaviorSubject<cartItem[]>([]);
  decoded: { CartId: number,AddressId : number, UserId: string};


  cartItems$ = this._cartItemsSubject.asObservable();

  get cartItems(): cartItem[] {
    const token = localStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
    }
    const cartId = this.decoded.CartId;
    const addressId = this.decoded.AddressId;
    const userId = this.decoded.UserId;
    return this._cartItems || this.cartItemService.fetchCartItemByCartId(cartId);
  }

  set cartItems(items: cartItem[]) {
    const token = localStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
    }
    const cartId = this.decoded.CartId;
    const addressId = this.decoded.AddressId;
    const userId = this.decoded.UserId;
    this._cartItems = items;
    this._cartItemsSubject.next(items);
    this.updateLocalStorage();
    this.cartItemService.addCartByCartId(items, cartId);
  }



  private updateLocalStorage(): void {
    localStorage.setItem('cartItems', JSON.stringify(this._cartItems));
  }
}
