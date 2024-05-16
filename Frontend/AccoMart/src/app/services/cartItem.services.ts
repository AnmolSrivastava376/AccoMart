import { Injectable } from "@angular/core";
import axios from "axios";
import { cartItem } from "../interfaces/cartItem";


@Injectable({
    providedIn:'root'
  })
export class cartItemService{

  addCartByCartId(cart: cartItem[], cartId: number) {
    return axios.post<cartItem[]>(`http://localhost:5239/ShoppingCartController/Add/CartItem?cartId=${cartId}`, cart);
  }

     fetchCartItemByCartId(cartId:number){
        return axios.get<cartItem[]>(`http://localhost:5239/ShoppingCartController/Get/CartItems?cartId=${cartId}`);
     }

     updateCartItemByProductIdAndQuantity(productId: number, quantity: number){
        return axios.put<cartItem>(`http://localhost:5239/ShoppingCartController/Update/CartItem?productId=${productId}&quantity=${quantity}`);
     }

     deleteCartItemByProductId(productId: number){
        return axios.delete<cartItem>(`http://localhost:5239/ShoppingCartController/Delete/CartItem?productId=${productId}`);
     }

}
