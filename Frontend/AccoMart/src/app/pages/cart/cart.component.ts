import { Component} from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { CartProductCardComponent } from '../../components/cart-product-card/cart-product-card.component';
import { CommonModule, NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { cartItemService } from '../../services/cartItem.services';
import { HttpClientModule } from '@angular/common/http';
import { cartItem } from '../../interfaces/cartItem';
import { jwtDecode } from 'jwt-decode';
import { addressService } from '../../services/address.service';
import { Address } from '../../interfaces/address';
import { ChangeDetectorRef } from '@angular/core';
import { DeliveryService } from '../../interfaces/deliveryService';
import { deliveryServices } from '../../services/delivery.service';

import { CartOrder } from '../../interfaces/placeOrder';
import { FormsModule } from '@angular/forms';
import { orderServices } from '../../services/order.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [NavbarComponent, CartProductCardComponent, CommonModule, HttpClientModule,FormsModule],
  providers : [cartItemService],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
  isVisible = false;
  selectedDeliveryId: number;
  toggleVisibility() {
    this.isVisible = !this.isVisible;
  }

  constructor(private router: Router, private cartItemService : cartItemService, private addressService: addressService,private deliveryService : deliveryServices,
    private orderService : orderServices, private cdr: ChangeDetectorRef) {}

  cart: cartItem[] = [{
    productId : 0,
    quantity : 0
  }];

  address: Address = {
    street : "",
    city : "",
    state : "",
    zipCode : "",
    phoneNumber : ""
  };

  delivery: DeliveryService[] = [{
    deliveryId : 0,
    imageUrl : "",
    serviceName : "",
    price : 0,
    deliveryDays : 0
  }];

  order: CartOrder = {
    userId : "",
    cartId : 0,
    addressId: 0,
    deliveryId : 0
  };

  decoded: { CartId: number,AddressId : number, UserId: string};

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
    }
    const cartId = this.decoded.CartId;
    const addressId = this.decoded.AddressId;
    const userId = this.decoded.UserId; // Add this line to get userId

    // Fetching cart items
    this.cartItemService.fetchCartItemByCartId(cartId)
    .then((response) => {
      this.cart = response.data;
    })
    .catch((error) => {
      console.error('Error fetching cart:', error);
    });

    // Fetching address
    this.addressService.getAddress(addressId)
    .then((response) => {
      console.log(response.data);
      this.address = response.data;
      //this.cdr.detectChanges();
      console.log(this.address);
      console.log(this.address.city);
      console.log(this.address.street || this.address.city || this.address.state || this.address.zipCode || this.address.phoneNumber)
    })
    .catch((error) => {
      console.error('Error fetching address:', error);
    });

    // Fetching delivery
    this.deliveryService.getDeliveryServices()
    .then((response) => {
      //console.log(this.delivery.length);
      this.delivery = response.data;
    })
    .catch((error) => {
      console.error('Error fetching delivery services:', error);
    });
    console.log(this.delivery.length);
  }

  placeOrder() {
    this.orderService.placeOrderByCart(this.decoded.UserId, this.decoded.CartId, this.decoded.AddressId, 6)
      .then((response: { data: string; }) => {
        const result: string = response.data;
        window.location.href = result;
        console.log(result);
      })
      .catch((error: any) => {
        console.error('Error placing order:', error);
      });
  }
}
