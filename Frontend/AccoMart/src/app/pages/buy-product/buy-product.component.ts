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
import { orderServices } from '../../services/order.service';
import { CartService } from '../../services/cart.services';
import { Subscription } from 'rxjs';
import { PaymentMethodComponent } from '../../components/payment-method/payment-method.component';
import { ChangeAddressComponent } from '../../components/change-address/change-address.component';
import { ChangeServiceComponent } from '../../components/change-service/change-service.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-buy-product',
  standalone: true,
  imports: [NavbarComponent, CartProductCardComponent, CommonModule, HttpClientModule,FormsModule,PaymentMethodComponent, ChangeAddressComponent, ChangeServiceComponent],
  templateUrl: './buy-product.component.html',
  styleUrl: './buy-product.component.css'
})
export class BuyProductComponent {
  isVisible = false;
  selectedDeliveryId: number;
  cartItemLength=0;
  private cartSubscription: Subscription;
  constructor(private router: Router, private cartItemService : cartItemService, private addressService: addressService,private deliveryService : deliveryServices,
    private orderService : orderServices, private cdr: ChangeDetectorRef, private cartService: CartService) {}

  cart: cartItem[] = [];
  clickedIndex=0;
  address: Address;
  delivery: DeliveryService[] = [];
  activeDeliveryIndex=0;
  activeDeliveryService: DeliveryService;

  decoded: { CartId: number,AddressId : number, UserId: string};

  ngOnInit(): void {
    this.cartItemLength = this.cartService.fetchQuantityInCart();
    this.cart = this.cartService.fetchCart();
    this.cartSubscription = this.cartService.getCartItems$().subscribe(
      items=>{
        this.cart = items
      }
    )
    this.cartSubscription = this.cartService.getCartItems$().subscribe(
      (      items: string | any[]) => {
        this.cartItemLength = items.length;
      }
    );
    const token = localStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
    }
    const cartId = this.decoded.CartId;
    const addressId = this.decoded.AddressId;
    const userId = this.decoded.UserId;

    // Fetching address
    this.addressService.getAddress(addressId)
    .then((response) => {
      console.log(response.data);
      this.address = response.data;
    })
    .catch((error) => {
      console.error('Error fetching address:', error);
    });

    // Fetching delivery
    this.deliveryService.getDeliveryServices()
    .then((response) => {
      this.delivery = response.data;
      if(this.delivery)
      this.activeDeliveryService = this.delivery[this.activeDeliveryIndex]
    })
    .catch((error) => {
      console.error('Error fetching delivery services:', error);
    });
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
  updateActiveDeliveryService(service: DeliveryService) {
    this.activeDeliveryService = service;
  }
  updateActiveDeliveryIndex(index:number){
    this.activeDeliveryIndex = index;
    this.activeDeliveryService = this.delivery[this.activeDeliveryIndex]
  }
  toggleVisibility(clickedIndex: number) {
    this.clickedIndex = clickedIndex
    this.isVisible = !this.isVisible;
  }
}
