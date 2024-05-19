import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { CartOrder } from "../interfaces/placeOrder";

@Injectable({
    providedIn:'root'
})
export class orderService {

    constructor(private http: HttpClient) {}

    placeOrderByCart(userId: string, cartId: number, addressId: number, deliveryId: number): Observable<string> {
        return this.http.post<string>(`http://localhost:5239/OrderController/PlaceOrderByCart`, {
            userId,
            cartId,
            addressId,
            deliveryId
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
