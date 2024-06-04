import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CartOrder } from '../interfaces/placeOrder';
import { Order } from '../interfaces/order';
import { cartItem } from '../interfaces/cartItem';
import { ProductOrder } from '../interfaces/productOrder';
import { PlaceOrderResponse } from '../interfaces/PlaceOrderResponse';
import { Item } from '../interfaces/item';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  constructor(private http: HttpClient) {}

  baseUrl = environment.serverUrl + 'OrderController/';

  placeOrderByCart(cartOrder: CartOrder): Observable<PlaceOrderResponse> {
    return this.http.post<PlaceOrderResponse>(
      `${this.baseUrl}PlaceOrderByCart`,
      cartOrder
    );
  }

  placeOrderByProduct(
    productOrder: ProductOrder
  ): Observable<PlaceOrderResponse> {
    return this.http.post<PlaceOrderResponse>(
      `${this.baseUrl}PlaceOrderByProduct`,
      productOrder
    );
  }

  fetchAllOrders(userId: string): Observable<Order[]> {
    return this.http.get<Order[]>(
      `${this.baseUrl}FetchAllOrders/${userId}`
    );
  }

  fetchOrderByOrderId(orderId: number): Observable<cartItem[]> {
    return this.http.get<cartItem[]>(
      `${this.baseUrl}GetCartItemsByOrderId/${orderId}`
    );
  }

  async cancelOrder(orderId: number, items: Item[]): Promise<any> {
    return this.http.post(
      `${this.baseUrl}Order/Cancel/${orderId}`,
      items
    ).toPromise();
  }
}
