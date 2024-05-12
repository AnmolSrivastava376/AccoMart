import { Time } from "@angular/common";
import { Item } from "./item";

export interface Order{
    orderId: number,
    orderDate: Date,
    userId: number,
    address: string,
    orderAmount: number,
    orderTime: Time,
    itemArray: Item[],
    deliveryServiceId: number,
    isDelivered: boolean,
    expectedDate: Date,
    isCancelled : boolean,
}