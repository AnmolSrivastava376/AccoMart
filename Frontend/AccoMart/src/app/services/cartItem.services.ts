import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { cartItem } from '../interfaces/cartItem';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class cartItemService {

  constructor(private http: HttpClient) {
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
    return this._cartItems
  }

  set cartItems(items: cartItem[]) {
    const token = localStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
    }
    const cartId = this.decoded.CartId;
    this._cartItems = items;
    this._cartItemsSubject.next(items);
    this.updateLocalStorage();
    this.addCartByCartId(this._cartItems, cartId);
  }
  private updateLocalStorage(): void {
    localStorage.setItem('cartItems', JSON.stringify(this._cartItems));
  }

  addCartByCartId(cart: cartItem[], cartId: number): Observable<cartItem[]> {
    return this.http.post<cartItem[]>(
      `http://localhost:5239/ShoppingCartController/Add/CartItem?cartId=${cartId}`,
      cart
    );
  }

  fetchCartItemByCartId(cartId: number): Observable<cartItem[]> {
    return this.http.get<cartItem[]>(
      `http://localhost:5239/ShoppingCartController/Get/CartItems?cartId=${cartId}`
    );
  }

  updateCartItemByProductIdAndQuantity(productId: number, quantity: number): Observable<cartItem> {
    return this.http.put<cartItem>(
      `http://localhost:5239/ShoppingCartController/Update/CartItem?productId=${productId}&quantity=${quantity}`,
      null // Pass null as the body since this endpoint doesn't require a request body
    );
  }

  deleteCartItemByProductId(productId: number): Observable<cartItem> {
    return this.http.delete<cartItem>(
      `http://localhost:5239/ShoppingCartController/Delete/CartItem?productId=${productId}`
    );
  }


}
