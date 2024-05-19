import { Component} from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { CartProductCardComponent } from '../../components/cart-product-card/cart-product-card.component';
import { CommonModule, NgIf } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { cartItemService } from '../../services/cartItem.services';
import { HttpClientModule } from '@angular/common/http';
import { cartItem } from '../../interfaces/cartItem';
import { jwtDecode } from 'jwt-decode';
import { addressService } from '../../services/address.service';
import { Address } from '../../interfaces/address';
import { ChangeDetectorRef } from '@angular/core';
import { DeliveryService } from '../../interfaces/deliveryService';
import { deliveryService } from '../../services/delivery.service';
import { orderService } from '../../services/order.service';
import { cartService } from '../../services/cart.services';
import { Subscription } from 'rxjs';
import { PaymentMethodComponent } from '../../components/payment-method/payment-method.component';
import { ChangeAddressComponent } from '../../components/change-address/change-address.component';
import { ChangeServiceComponent } from '../../components/change-service/change-service.component';
import { FormsModule } from '@angular/forms';
import { Product } from '../../interfaces/product';
import { productService } from '../../services/product.services';

@Component({
  selector: 'app-buy-product',
  standalone: true,
  imports: [NavbarComponent, CartProductCardComponent, CommonModule, HttpClientModule,FormsModule,PaymentMethodComponent, ChangeAddressComponent, ChangeServiceComponent],
  providers : [cartItemService, addressService, deliveryService,productService,orderService,cartService],
  templateUrl: './buy-product.component.html',
  styleUrl: './buy-product.component.css'
})
export class BuyProductComponent {
  isVisible = false;
  selectedDeliveryId: number;
  cartItemLength=0;
  private cartSubscription: Subscription;
  constructor(private router: Router, private cartItemService : cartItemService, private addressService: addressService,private deliveryService : deliveryService, private productService: productService,
    private orderService : orderService, private cdr: ChangeDetectorRef, private cartService: cartService,private route : ActivatedRoute) {}

  cart: cartItem[] = [];
  clickedIndex=0;
  address: Address;
  delivery: DeliveryService[] = [];
  activeDeliveryIndex=0;
  activeDeliveryService: DeliveryService;
  products: Product[] = [];
  decoded: { CartId: number,AddressId : number, UserId: string};
  selectedProductId: number;

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.selectedProductId = +params['productId'];
    });
    this.cartService.addToCart(this.selectedProductId);
    this.cartItemLength = this.cartService.fetchQuantityInCart();
    this.cart = this.cartService.fetchCart();
    this.cartSubscription = this.cartService.getCartItems$().subscribe(
      items=>{
        this.cart = items;
        this.cartItemLength = items.length;
        this.cart.forEach(item=>{
          this.productService.fetchProductById(item.productId).then((response)=>{
            this.products.push(response.data)
          })
        })
      }
    )
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
      (response: DeliveryService[]) => {
        this.delivery = response;
        if (this.delivery) {
          this.activeDeliveryService = this.delivery[this.activeDeliveryIndex];
        }
      },
      (error: any) => {
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
      (response: string) => {
        window.location.href = response;
        console.log(response);
      },
      (error: any) => {
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
