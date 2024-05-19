import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { cartItem } from '../interfaces/cartItem';

@Injectable({
  providedIn: 'root',
})
export class cartItemService {

  constructor(private http: HttpClient) {}

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
