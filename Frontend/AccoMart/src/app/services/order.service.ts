import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CartOrder } from '../interfaces/placeOrder';
import { Order } from '../interfaces/order';
import { cartItem } from '../interfaces/cartItem';
import { ProductOrder } from '../interfaces/productOrder';
import { PlaceOrderResponse } from '../interfaces/PlaceOrderResponse';

@Injectable({
  providedIn: 'root',
})
export class orderService {
  constructor(private http: HttpClient) {}

  placeOrderByCart(cartOrder: CartOrder): Observable<PlaceOrderResponse> {
    return this.http.post<PlaceOrderResponse>(
      `http://localhost:5239/OrderController/PlaceOrderByCart`,
      cartOrder
    );
  }

  placeOrderByProduct(
    productOrder: ProductOrder
  ): Observable<PlaceOrderResponse> {
    return this.http.post<PlaceOrderResponse>(
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

  cancelOrder(orderId:number):Observable<any>{
    console.log(orderId);

    return this.http.delete<any>(
      `http://localhost:5239/OrderController/Order/Cancel/${orderId}`
    ); 

  }
}