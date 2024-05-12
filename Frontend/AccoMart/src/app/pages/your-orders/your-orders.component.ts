import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { OrderCardComponent } from '../../components/order-card/order-card.component';
import orders from './sampleData'
import { Order } from '../../interfaces/order';

@Component({
  selector: 'app-your-orders',
  standalone: true,
  imports: [CommonModule, NavbarComponent, OrderCardComponent],
  templateUrl: './your-orders.component.html',
  styleUrl: './your-orders.component.css'
})
export class YourOrdersComponent {
  orders:Order[]=orders
  currentOrders: Order[];
  historyOrders: Order[];
  constructor() {
    // Split orders into current and history based on isDelivered property
    this.currentOrders = this.orders.filter(order => !order.isDelivered && !order.isCancelled);
    this.historyOrders = this.orders.filter(order => order.isDelivered || order.isCancelled);
  }
}
