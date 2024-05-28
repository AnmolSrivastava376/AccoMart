import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { CartProductCardComponent } from '../../components/cart-product-card/cart-product-card.component';
import { CommonModule, NgIf } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { cartItem } from '../../interfaces/cartItem';
import { jwtDecode } from 'jwt-decode';
import { addressService } from '../../services/address.service';
import { Address } from '../../interfaces/address';
import { DeliveryService } from '../../interfaces/deliveryService';
import { deliveryService } from '../../services/delivery.service';
import { orderService } from '../../services/order.service';
import { CartService } from '../../services/cart.services';
import { PaymentMethodComponent } from '../../components/payment-method/payment-method.component';
import { ChangeAddressComponent } from '../../components/change-address/change-address.component';
import { ChangeServiceComponent } from '../../components/change-service/change-service.component';
import { FormsModule } from '@angular/forms';
import { Product } from '../../interfaces/product';
import { productService } from '../../services/product.services';
import { ToastrService } from 'ngx-toastr';
import { LoaderComponent } from '../../components/loader/loader.component';
import { BuyNowService } from '../../services/buy-now.service';
import { BuyNowProductCardComponent } from '../../components/buy-now-product-card/buy-now-product-card.component';
import { ProductOrder } from '../../interfaces/productOrder';

@Component({
  selector: 'app-buy-product',
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
    BuyNowProductCardComponent,
    HttpClientModule,
  ],
  providers: [
    addressService,
    deliveryService,
    productService,
    orderService,
    CartService,
    LoaderComponent,
    ToastrService,
  ],
  templateUrl: './buy-product.component.html',
  styleUrl: './buy-product.component.css',
})
export class BuyProductComponent {
  isVisible = false;
  selectedDeliveryId: number;
  spinLoader: boolean = false;
  cartItemLength = 0;
  cart: cartItem[] = [];
  clickedIndex = 0;
  address: Address[];
  activeAddress: Address;
  delivery: DeliveryService[] = [];
  activeDeliveryIndex = 0;
  activeDeliveryService: DeliveryService;
  products: Product[] = [];
  decoded: { CartId: number; AddressId: number; UserId: string };
  selectedProductId: number;
  productPrice: number = 0;
  productOrder: ProductOrder = {
    userId: '',
    productId: 0,
    addressId: 0,
    deliveryId: 0,
    quantity: 0,
  };
  cartId: number;
  addressId: number;
  userId: string;
  newCartItem: cartItem | null;
  isLoading = false;
  constructor(
    private addressService: addressService,
    private deliveryService: deliveryService,
    private productService: productService,
    private buyNowService: BuyNowService,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private orderService: orderService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.selectedProductId = +params['productId'];
    });
    this.newCartItem = this.buyNowService.getProductFromLocalStorage();
    if (this.newCartItem) {
      this.cart = [...this.cart, this.newCartItem];
      this.cartItemLength = 1;
    }
    const token = localStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
      this.cartId = this.decoded.CartId;
      this.addressId = 0;
      this.userId = this.decoded.UserId;
    }
    this.productOrder.addressId = this.addressId;
    this.productOrder.userId = this.userId;
    this.productOrder.productId = this.selectedProductId;
    this.productOrder.quantity = 1;
    this.productService
      .fetchProductById(this.productOrder.productId)
      .subscribe((response) => (this.productPrice = response.productPrice));
    this.addressService
      .getAddressByUserId(this.userId)
      .subscribe((response: any) => {
        if (response.response.length > 0) {
          this.address = response.response;
          this.activeAddress = this.address[0];
          this.productOrder.addressId = this.activeAddress.addressId;
        } else {
          console.error('Failed to retrieve addresses:', response.message);
          this.toastr.error('Failed to retrieve addresses');
        }
      });
    this.deliveryService.getDeliveryServices().subscribe((response: any) => {
      if (response.isSuccess) {
        this.delivery = response.response;
        if (this.delivery && this.delivery.length > 0) {
          this.activeDeliveryService = this.delivery[0];
          this.productOrder.deliveryId = this.activeDeliveryService.dServiceId;
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
    total = this.productPrice * this.productOrder.quantity;
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

  placeOrderByProduct() {
    if (this.productOrder.addressId === 0) {
      alert('You need to provide an address');
    } else {
      if (!this.isLoading) {
        this.isLoading = true;
        this.orderService.placeOrderByProduct(this.productOrder).subscribe({
          next: (response) => {
            window.location.href = response.stripeUrl;
          },
          error: (error) => {
            console.error('Error placing order:', error);
          }
        });
      }
    }
  }

  updateActiveDeliveryIndex(index: number) {
    this.activeDeliveryIndex = index;
    this.activeDeliveryService = this.delivery[this.activeDeliveryIndex];
    this.productOrder.deliveryId = this.activeDeliveryService.dServiceId;
  }

  updateActiveAddress(address: Address) {
    this.activeAddress = address;
    this.productOrder.addressId = this.activeAddress.addressId;
  }

  toggleVisibility(clickedIndex: number) {
    this.clickedIndex = clickedIndex;
    this.isVisible = !this.isVisible;
  }

  handleQuantityChange(event: any) {
    this.productOrder.quantity = event.quantity;
  }
}
