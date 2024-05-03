import { Time } from "@angular/common";

export interface order{
    orderId: number,
    orderDate:Date,
    userId: number,
    addressId: number,
    orderAmount: number,
    orderTime: Time,
    productId: number, 
    cartId: number,
    deliveryServiceId: number,
    isCancelled : BinaryType //I am not sure of this datatype as in sql  datatype is bit
}