import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { OrderCardComponent } from '../../components/order-card/order-card.component';
import { Order } from '../../interfaces/order';
import { orderService } from '../../services/order.service';
import { jwtDecode } from 'jwt-decode';
import { HttpClientModule } from '@angular/common/http';
import { deliveryService } from '../../services/delivery.service';
import { addressService } from '../../services/address.service';

@Component({
  selector: 'app-your-orders',
  standalone: true,
  imports: [CommonModule, NavbarComponent, OrderCardComponent, HttpClientModule],
  templateUrl: './your-orders.component.html',
  styleUrl: './your-orders.component.css',
  providers: [orderService, deliveryService, addressService]
})
export class YourOrdersComponent implements OnInit {
  orders:Order[] = []
  currentOrders: Order[]=[];
  historyOrders: Order[]=[];
  userId: string
  decoded: any
  constructor(private orderService: orderService, private deliveryService: deliveryService, private addressService: addressService) {
  }
  ngOnInit(): void {
    const token = localStorage.getItem('token')
    if(token){
      this.decoded = jwtDecode(token);
      this.userId = this.decoded.UserId;
      this.orderService.fetchAllOrders(this.userId).subscribe(
        response=>{
          this.orders = response;
          this.addAddresses();
          this.sortOrders();
        }
      )
    }
  }
  addAddresses(){
    this.orders.forEach(order=>{
      this.addressService.getAddressByAddressId(order.addressId).subscribe(
        response=>{
          order.address = response.street + ", " + response.city + response.state + " - " + response.zipCode
        }
      )
    })
  }
  sortOrders(){
    const currentDate = new Date();
    this.orders.forEach(order=>{
      let deliveryDays=0;
      this.deliveryService.getDeliveryDate(order.deliveryServiceID).subscribe(
        response=>{
          deliveryDays = response;
          const orderDate = new Date(order.orderDate);
          order.expectedDate = new Date(orderDate.getTime() + (deliveryDays * 24 * 60 * 60 * 1000));
          if (currentDate > order.expectedDate) {
            order.isDelivered = true;
          } else {
            order.isDelivered = false;
          }
          if (!order.isCancelled && !order.isDelivered) {
            this.currentOrders.push(order);
          } else {
            this.historyOrders.push(order);
          }
        }
      )
    });
  }
}
