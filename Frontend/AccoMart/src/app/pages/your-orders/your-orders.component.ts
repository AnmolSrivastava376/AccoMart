import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { OrderCardComponent } from '../../components/order-card/order-card.component';
import { Order } from '../../interfaces/order';
import { orderService } from '../../services/order.service';
import { jwtDecode } from 'jwt-decode';
import { HttpClientModule } from '@angular/common/http';
import { deliveryService } from '../../services/delivery.service';

@Component({
  selector: 'app-your-orders',
  standalone: true,
  imports: [CommonModule, NavbarComponent, OrderCardComponent, HttpClientModule],
  templateUrl: './your-orders.component.html',
  styleUrl: './your-orders.component.css',
  providers: [orderService, deliveryService]
})
export class YourOrdersComponent implements OnInit {
  orders:Order[] = []
  currentOrders: Order[];
  historyOrders: Order[];
  userId: string
  decoded: any
  constructor(private orderService: orderService, private deliveryService: deliveryService) {
  }
  ngOnInit(): void {
    const token = localStorage.getItem('token')
    if(token){
      this.decoded = jwtDecode(token);
      this.userId = this.decoded.UserId;
      this.orderService.fetchAllOrders(this.userId).subscribe(
        response=>{
          this.orders = response;
          this.sortOrders();
        }
      )
    }
  }
  sortOrders(){
    const currentDate = new Date();
    this.orders.forEach(order=>{
      const deliveryDays = this.deliveryService.getDeliveryDate(order.deliveryServiceId)
      const orderDate = new Date(order.orderDate);
      // if(orderDate + 3){

      // }
    });
  }
}
