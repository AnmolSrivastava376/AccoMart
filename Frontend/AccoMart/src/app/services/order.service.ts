import { Injectable } from "@angular/core";
import axios from "axios";
import { CartOrder } from "../interfaces/placeOrder";


@Injectable({
    providedIn:'root'
  })
export class orderServices{

      placeOrderByCart(userId : string, cartId : number, addressId : number, deliveryId : number){
        return axios.post<string>(`http://localhost:5239/OrderController/PlaceOrderByCart?userId=${userId}&cartId=${cartId}&addressId=${addressId}&deliveryId=${deliveryId}`);
     }


}
