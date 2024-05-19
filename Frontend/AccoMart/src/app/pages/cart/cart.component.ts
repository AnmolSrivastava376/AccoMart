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
import { deliveryService } from '../../services/delivery.service';
import { FormsModule } from '@angular/forms';
import { orderService } from '../../services/order.service';
import { cartService } from '../../services/cart.services';
import { Subscription } from 'rxjs';
import { PaymentMethodComponent } from '../../components/payment-method/payment-method.component';
import { ChangeAddressComponent } from '../../components/change-address/change-address.component';
import { ChangeServiceComponent } from '../../components/change-service/change-service.component';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [NavbarComponent, CartProductCardComponent, CommonModule,FormsModule,PaymentMethodComponent, ChangeAddressComponent, ChangeServiceComponent,HttpClientModule],
  providers : [addressService, deliveryService,productService,orderService,cartService,cartItemService],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
  isVisible = false;
  selectedDeliveryId: number;
  cartItemLength=0;
  private cartSubscription: Subscription;
  constructor(private router: Router, private addressService: addressService,private deliveryService : deliveryService, private productService: productService,private orderService : orderService, private cartService: cartService) {}

  cart: cartItem[] = [];
  clickedIndex=0;
  address: Address;
  delivery: DeliveryService[] = [];
  activeDeliveryIndex=0;
  activeDeliveryService: DeliveryService;
  products: Product[] = [];
  decoded: { CartId: number,AddressId : number, UserId: string};

  ngOnInit(): void {
    this.cartItemLength = this.cartService.fetchQuantityInCart();
    this.cart = this.cartService.fetchCart();
    this.cartSubscription = this.cartService.getCartItems$().subscribe(
      items => {
        this.cart = items;
        this.cartItemLength = items.length;
        // Use a combination of map and forkJoin to fetch products in parallel
        const productRequests = items.map(item =>
          this.productService.fetchProductById(item.productId)
       );
      });
    const token = localStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
    }
    const cartId = this.decoded.CartId;
    const addressId = this.decoded.AddressId;
    const userId = this.decoded.UserId;

    // Fetching address
    this.addressService.getAddress(addressId)
  .subscribe(
    (response) => {
      console.log(response);
      this.address = response;
    },
    (error) => {
      console.error('Error fetching address:', error);
    }
  );


    // Fetching delivery
    this.deliveryService.getDeliveryServices()
    .subscribe(
      (response) => {
        this.delivery = response;
        if (this.delivery) {
          this.activeDeliveryService = this.delivery[this.activeDeliveryIndex];
        }
      },
      (error) => {
        console.error('Error fetching delivery services:', error);
      }
    );

  }
  getCartTotal(): number {
    let total = 0;
    this.cart.forEach((item, index) => {
      total += this.products[index].productPrice * item.quantity;
    });
    return total;
  }
  getDeliveryCharges(): number {
    return this.activeDeliveryService ? this.activeDeliveryService.price : 0;
  }
  getDiscounts(): number {
    return 5.00;
  }
  getTaxes(): number{
    return 10;
  }
  getGrandTotal(): number {
    return this.getCartTotal() + this.getDeliveryCharges() + this.getTaxes() - this.getDiscounts();
  }
  placeOrder() {
    this.orderService.placeOrderByCart(this.decoded.UserId, this.decoded.CartId, this.decoded.AddressId, 6)
    .subscribe(
      (response) => {
        window.location.href = response;
        console.log(response);
      },
      (error) => {
        console.error('Error placing order:', error);
      }
    );
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
