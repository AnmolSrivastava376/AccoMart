import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { CartOrder, ProductOrder } from "../interfaces/placeOrder";
import { stripeDto } from "../interfaces/StripeDto";

@Injectable({
    providedIn:'root'
})
export class orderService {

    constructor(private http: HttpClient) {}

    placeOrderByCart(cartOrder :CartOrder): Observable<stripeDto> {
        return this.http.post<stripeDto>(`http://localhost:5239/OrderController/PlaceOrderByCart`, {
          cartOrder
        });
    }

    placeOrderByProduct(productOrder : ProductOrder): Observable<stripeDto> {
        return this.http.post<stripeDto>(`http://localhost:5239/OrderController/PlaceOrderByProduct`, {
          productOrder
        });
    }
}
