import { Time } from "@angular/common";
import { Item } from "./item";

export interface Order{
    orderId: number,
    orderDate: Date,
    userId: string,
    address: string,
    addressId: number,
    orderAmount: number,
    orderTime: Time,
    itemArray: Item[],
    deliveryServiceID: number,
    isDelivered: boolean,
    expectedDate: Date,
    isCancelled : boolean,
}