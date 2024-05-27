import { Component} from '@angular/core';
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
import { Subscription } from 'rxjs';
import { PaymentMethodComponent } from '../../components/payment-method/payment-method.component';
import { ChangeAddressComponent } from '../../components/change-address/change-address.component';
import { ChangeServiceComponent } from '../../components/change-service/change-service.component';
import { FormsModule } from '@angular/forms';
import { Product } from '../../interfaces/product';
import { productService } from '../../services/product.services';
import { ProductOrder } from '../../interfaces/placeOrder';
import { ToastrService } from 'ngx-toastr';
import { LoaderComponent } from '../../components/loader/loader.component';

@Component({
  selector: 'app-buy-product',
  standalone: true,
  imports:
  [LoaderComponent,
    NavbarComponent,
    CartProductCardComponent,
    CommonModule,
    FormsModule,
    PaymentMethodComponent,
    ChangeAddressComponent,
    ChangeServiceComponent,
    HttpClientModule],
  providers : [addressService, deliveryService,productService,orderService,CartService],
  templateUrl: './buy-product.component.html',
  styleUrl: './buy-product.component.css'
})
export class BuyProductComponent {
  isVisible = false;
  selectedDeliveryId: number;
  spinLoader:boolean= false;
  cartItemLength=0;
  private cartSubscription: Subscription;
  constructor(private router: Router, private addressService: addressService,private deliveryService : deliveryService, private productService: productService,
    private orderService : orderService, private cartService: CartService,private route : ActivatedRoute, private toastr : ToastrService) {}

    cart: cartItem[] = [];
    clickedIndex = 0;
    address: Address[];
    activeAddress: Address;
    delivery: DeliveryService[] = [];
    activeDeliveryIndex = 0;
    activeDeliveryService: DeliveryService;
    products: Product[] = [];
    decoded: { CartId: number,AddressId : number, UserId: string};
    selectedProductId: number;
    productOrder : ProductOrder = {
    userId : "",
    addressId: 0,
    deliveryId : 0,
    productId : 0
    }
    cartId: number;
    addressId: number;
    userId: string;


  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.selectedProductId = +params['productId'];
    });
    this.cartService.addToCart(this.selectedProductId);
    this.cartItemLength = JSON.parse(localStorage.getItem('cartItems')||"").length;
    const token = localStorage.getItem('token');
    if (token) {
      this.decoded = jwtDecode(token);
      this.cartId = this.decoded.CartId;
      this.addressId = 1;
      this.userId = this.decoded.UserId;
    }

    this.productOrder.addressId = this.addressId
    this.productOrder.userId = this.userId;
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
    this.addressService.getAddressByUserId(this.userId).subscribe((response: any) => {
      if (response.isSuccess) {
        this.address = response.response;
        this.activeAddress = this.address[0];
      } else {
        console.error('Failed to retrieve addresses:', response.message);
        this.toastr.error("Failed to retrieve addresses");
      }
    });


    // Fetching delivery
    this.deliveryService.getDeliveryServices().subscribe((response: any) => {
      if (response.isSuccess) {
      this.delivery = response.response;
      if (this.delivery && this.delivery.length > 0) {
        this.activeDeliveryService = this.delivery[0];
        this.productOrder.deliveryId = this.activeDeliveryService.dServiceId;
      }
    } else {
      console.error('Failed to retrieve delivery services:', response.message);
      this.toastr.error("Failed to retrieve delivery services");
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
    this.orderService.placeOrderByProduct(this.productOrder)
    .subscribe(
      (response) => {
        window.location.href = response.url;
      },
      (error) => {
        console.error('Error placing order:', error);
      }
    );
  }
  updateActiveDeliveryIndex(index: number) {
    this.activeDeliveryIndex = index;
    this.activeDeliveryService = this.delivery[this.activeDeliveryIndex];
    this.productOrder.deliveryId = this.activeDeliveryService.dServiceId;
  }
  updateActiveAddress(address:Address){
    this.activeAddress = address
  }
  toggleVisibility(clickedIndex: number) {
    this.clickedIndex = clickedIndex;
    this.isVisible = !this.isVisible;
  }
}
