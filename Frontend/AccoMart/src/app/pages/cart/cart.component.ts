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
import { Subscription } from 'rxjs';
import { PaymentMethodComponent } from '../../components/payment-method/payment-method.component';
import { ChangeAddressComponent } from '../../components/change-address/change-address.component';
import { ChangeServiceComponent } from '../../components/change-service/change-service.component';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';
import { CartOrder } from '../../interfaces/placeOrder';
import { ToastrService } from 'ngx-toastr';
import { LoaderComponent } from '../../components/loader/loader.component';
import { error } from 'highcharts';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [
    LoaderComponent,
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
    ToastrService,
  ],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent {
  isVisible = false;
  selectedDeliveryId: number;
  cartItemLength = 0;
  spinLoader: boolean = false;
  private cartSubscription: Subscription;
  constructor(
    private addressService: addressService,
    private deliveryService: deliveryService,
    private productService: productService,
    private orderService: orderService,
    private cartService: CartService,
    private toastr: ToastrService,
    private router : Router
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
    this.cartItemLength = JSON.parse(
      localStorage.getItem('cartItems') || ''
    ).length;
    const token = localStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
      this.cartId = this.decoded.CartId;
      this.addressId = 0;
      this.userId = this.decoded.UserId;
    }

    this.cartOrder.addressId = this.addressId;
    this.cartOrder.userId = this.userId;
    this.cartOrder.cartId = this.cartId;
    this.cartSubscription = this.cartService
      .getCartItems$()
      .subscribe((item) => {
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
    this.addressService
      .getAddressByUserId(this.userId)
      .subscribe((response: any) => {
        if (response.response.length > 0) {
          this.address = response.response;
          this.activeAddress = this.address[0];
          this.cartOrder.addressId = this.activeAddress.addressId;
        } else {
          console.error('Failed to retrieve addresses:', response.message);
          this.toastr.error('Failed to retrieve addresses');
        }
      });

    // Fetching delivery
    this.deliveryService.getDeliveryServices().subscribe((response: any) => {
      if (response.isSuccess) {
        this.delivery = response.response;
        if (this.delivery && this.delivery.length > 0) {
          this.activeDeliveryService = this.delivery[0];
          this.cartOrder.deliveryId = this.activeDeliveryService.dServiceId;
        }
      } else {
        console.error(
          'Failed to retrieve delivery services:',
          response.message
        );
        this.toastr.error('Failed to retrieve delivery services');
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
    let discount = 0;
    discount = (5 / 100) * this.getCartTotal();
    return +discount.toFixed(2);
  }

  getTaxes(): number {
    let totalAmount = 0;
    totalAmount =
      (18 / 100) * this.getCartTotal() +
      this.getDeliveryCharges() +
      this.getDiscounts();
    return +totalAmount.toFixed(2);
  }

  getGrandTotal(): number {
    let grandTotal = 0;
    grandTotal =
      this.getCartTotal() +
      this.getDeliveryCharges() +
      this.getTaxes() -
      this.getDiscounts();
    return +grandTotal.toFixed(2);
  }

  placeOrder() {
    if (this.cartOrder.addressId === 0) {
      alert('You need to provide an address');
    } else {
      if (!this.spinLoader) {
        this.spinLoader = true;
        this.orderService
          .placeOrderByCart(this.cartOrder)
          .subscribe({
            next: (response) => {
              this.spinLoader = false;
              if (response.stripeModel && response.stripeModel.stripeUrl) {
                window.location.href = response.stripeModel.stripeUrl;
            } else {
                console.error('Stripe URL not found in response:', response);
            }
            },
            error: (error)=>{
              this.spinLoader = false;
              this.toastr.error(error.error.message);
            }
          });

      }
    }
  }

  isValidUrl(url: string): boolean {
    const urlPattern = /^(https?:\/\/)?([\w.]+)\.([a-z]{2,})(\/\S*)?$/i;
    return urlPattern.test(url);
  }

  updateActiveDeliveryIndex(index: number) {
    this.activeDeliveryIndex = index;
    this.activeDeliveryService = this.delivery[this.activeDeliveryIndex];
    this.cartOrder.deliveryId = this.activeDeliveryService.dServiceId;
  }

  updateActiveAddress(address: Address) {
    this.activeAddress = address;
    this.cartOrder.addressId = address.addressId;
  }

  toggleVisibility(clickedIndex: number) {
    this.clickedIndex = clickedIndex;
    this.isVisible = !this.isVisible;
  }
}
