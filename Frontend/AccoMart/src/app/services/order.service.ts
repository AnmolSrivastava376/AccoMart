import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { CartOrder, ProductOrder } from "../interfaces/placeOrder";

@Injectable({
    providedIn:'root'
})
export class orderService {

    constructor(private http: HttpClient) {}

    placeOrderByCart(cartOrder :CartOrder): Observable<{url: string}> {
        return this.http.post<{url: string}>(`http://localhost:5239/OrderController/PlaceOrderByCart`, {
          cartOrder
        });
    }

    placeOrderByProduct(productOrder : ProductOrder): Observable<{url: string}> {
        return this.http.post<{url: string}>(`http://localhost:5239/OrderController/PlaceOrderByProduct`, {
          productOrder
        });
    }
}
