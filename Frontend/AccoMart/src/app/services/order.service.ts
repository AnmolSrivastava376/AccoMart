import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { CartOrder } from "../interfaces/placeOrder";
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

    placeOrderByProduct(userId: string, addressId: number, deliveryId: number, productId: number): Observable<string> {
        return this.http.post<string>(`http://localhost:5239/OrderController/PlaceOrderByProduct`, {
            userId,
            addressId,
            deliveryId,
            productId
        });
    }
}
