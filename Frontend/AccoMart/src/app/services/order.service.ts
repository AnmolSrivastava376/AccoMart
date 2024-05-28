import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CartOrder } from '../interfaces/placeOrder';
import { Order } from '../interfaces/order';
import { cartItem } from '../interfaces/cartItem';
import { ProductOrder } from '../interfaces/productOrder';

@Injectable({
  providedIn: 'root',
})
export class orderService {
  constructor(private http: HttpClient) {}

  placeOrderByCart(cartOrder: CartOrder): Observable<{ stripeUrl: string }> {
    return this.http.post<{ stripeUrl: string }>(
      `http://localhost:5239/OrderController/PlaceOrderByCart`,
      cartOrder
    );
  }

  placeOrderByProduct(
    productOrder: ProductOrder
  ): Observable<{ stripeUrl: string }> {
    return this.http.post<{ stripeUrl: string }>(
      `http://localhost:5239/OrderController/PlaceOrderByProduct`,
      productOrder
    );
  }

  fetchAllOrders(userId: string): Observable<Order[]> {
    return this.http.get<Order[]>(
      `http://localhost:5239/OrderController/FetchAllOrders/${userId}`
    );
  }

  fetchOrderByOrderId(orderId: number): Observable<cartItem[]> {
    return this.http.get<cartItem[]>(
      `http://localhost:5239/OrderController/GetCartItemsByOrderId/${orderId}`
    );
  }
}