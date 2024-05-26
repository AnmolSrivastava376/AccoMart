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
import { Product } from '../../interfaces/product';
import { Item } from '../../interfaces/item';
import { cartItem } from '../../interfaces/cartItem';
import { productService } from '../../services/product.services';
import { invoiceService } from '../../services/invoiceService';
import { LoaderComponent } from '../../components/loader/loader.component';

@Component({
  selector: 'app-your-orders',
  standalone: true,
  imports: [CommonModule, NavbarComponent, OrderCardComponent, HttpClientModule,LoaderComponent],
  templateUrl: './your-orders.component.html',
  styleUrl: './your-orders.component.css',
  providers: [orderService, deliveryService, addressService, productService]
})
export class YourOrdersComponent implements OnInit {
  orders:Order[] = []
  currentOrders: Order[]=[];
  historyOrders: Order[]=[];
  userId: string
  decoded: any
  constructor(private orderService: orderService, private deliveryService: deliveryService, private addressService: addressService, private productService: productService) {
  }
  ngOnInit(): void {
    const token = localStorage.getItem('token')
    if(token){
      this.decoded = jwtDecode(token);
      this.userId = this.decoded.UserId;
      this.orderService.fetchAllOrders(this.userId).subscribe(
        response=>{
          this.orders = response;
          this.addProducts();
          this.addAddresses();
          this.sortOrders();
        }
      )
    }
  }
  addProducts(){
    this.orders.forEach(order=>{
      this.orderService.fetchOrderByOrderId(order.orderId).subscribe(
        response=>{
          order.itemArray = this.fetchProductsByCart(response);
        }
      )
    })
  }
  fetchProductsByCart(cartItem: cartItem[]):Item[]{
    const products:Item[]=[];
    cartItem.forEach(item=>{
      this.productService.fetchProductById(item.productId).subscribe(
        response=>{
          products.push({"product": response, "quantity": item.quantity})
        }
      )
    })
    return products
  }
  addAddresses(){
    this.orders.forEach(order=>{
      this.addressService.getAddressByAddressId(order.addressId).subscribe(
        response=>{
          order.address = response.street + ", " + response.city+ ", " + response.state + " - " + response.zipCode
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



