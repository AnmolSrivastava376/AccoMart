import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { CartProductCardComponent } from '../../components/cart-product-card/cart-product-card.component';
import { CommonModule, NgIf } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { cartItem } from '../../interfaces/cartItem';
import { jwtDecode } from 'jwt-decode';
import { addressService } from '../../services/address.service';
import { Address } from '../../interfaces/address';
import { DeliveryService } from '../../interfaces/deliveryService';
import { deliveryService } from '../../services/delivery.service';
import { FormsModule } from '@angular/forms';
import { orderService } from '../../services/order.service';
import { CartService } from '../../services/cart.services';
import { Subscription, forkJoin } from 'rxjs';
import { PaymentMethodComponent } from '../../components/payment-method/payment-method.component';
import { ChangeAddressComponent } from '../../components/change-address/change-address.component';
import { ChangeServiceComponent } from '../../components/change-service/change-service.component';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';
import { CartOrder } from '../../interfaces/placeOrder';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [
    NavbarComponent,
    CartProductCardComponent,
    CommonModule,
    FormsModule,
    PaymentMethodComponent,
    ChangeAddressComponent,
    ChangeServiceComponent,
    HttpClientModule,
  ],
  providers: [
    addressService,
    deliveryService,
    productService,
    orderService,
    CartService,
  ],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent {
  isVisible = false;
  selectedDeliveryId: number;
  cartItemLength = 0;
  private cartSubscription: Subscription;
  constructor(
    private addressService: addressService,
    private deliveryService: deliveryService,
    private productService: productService,
    private orderService: orderService,
    private cartService: CartService
  ) {}

  cart: cartItem[] = [];
  clickedIndex = 0;
  address: Address[];
  activeAddress: Address;
  delivery: DeliveryService[] = [];
  activeDeliveryIndex = 0;
  activeDeliveryService: DeliveryService;
  products: Product[] = [];
  decoded: { CartId: number; AddressId: number; UserId: string };
  cartOrder: CartOrder = {
    userId: '',
    cartId: 0,
    addressId: 0,
    deliveryId: 0,
  };
  cartId: number;
  addressId: number;
  userId: string;
  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
      this.cartId = this.decoded.CartId;
      this.addressId = 1;
      this.userId = this.decoded.UserId;
    }

    this.cartOrder.addressId = this.addressId
    this.cartOrder.userId = this.userId;
    this.cartOrder.cartId = this.cartId;
    this.cartSubscription = this.cartService.getCartItems$().subscribe((item) => {
        this.cart = item;
        this.cartItemLength = item.length;
        item.forEach((cartItem) => {
          this.productService
            .fetchProductById(cartItem.productId)
            .subscribe((product) => {
              this.products.push(product);
            });
        });
      });
    // Fetching address
    this.addressService.getAddress(this.userId).subscribe((response) => {
      this.address = response;
      this.activeAddress = this.address[0]
    });
    // Fetching delivery
    this.deliveryService.getDeliveryServices().subscribe((response) => {
      this.delivery = response;
      if (this.delivery) {
        this.activeDeliveryService = this.delivery[this.activeDeliveryIndex];
        this.cartOrder.deliveryId = this.activeDeliveryService.dServiceId;
      }
    });
  }
  

 
  getCartTotal(): number {
    let total = 0;
    if (this.cartItemLength > 0) {
      this.cart.forEach((item, index) => {
        total += this.products[index]?.productPrice * item.quantity;
      });
    }
    return total;
  }
  getDeliveryCharges(): number {
    return this.activeDeliveryService ? this.activeDeliveryService.price : 0;
  }
  getDiscounts(): number {
    return 5.0;
  }
  getTaxes(): number {
    return 10;
  }
  getGrandTotal(): number {
    return this.getCartTotal() + this.getDeliveryCharges() + this.getTaxes() - this.getDiscounts();
  }

  placeOrder() {
    console.log(this.cartOrder)
    this.orderService.placeOrderByCart(this.cartOrder).subscribe(
      response=>{
        console.log(response)
      }
    );
  }
  updateActiveDeliveryIndex(index: number) {
    this.activeDeliveryIndex = index;
    this.activeDeliveryService = this.delivery[this.activeDeliveryIndex];
    this.cartOrder.deliveryId = this.activeDeliveryService.dServiceId;
  }
  toggleVisibility(clickedIndex: number) {
    this.clickedIndex = clickedIndex;
    this.isVisible = !this.isVisible;
  }

  // onAddressAdded(address: Address) {
  //   this.addresses.push(address); // Add the newly added address to the array
  //   this.isVisible = false; // Close the change address form
  // }
}
