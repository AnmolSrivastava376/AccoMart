import { HttpClientModule } from '@angular/common/http';
import { Component, Input } from '@angular/core';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';
import { CartService } from '../../services/cart.services';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { LoaderComponent } from '../loader/loader.component';

@Component({
  selector: 'app-cart-product-card',
  standalone: true,
  providers: [productService, CartService],
  templateUrl: './cart-product-card.component.html',
  styleUrl: './cart-product-card.component.css',
  imports: [HttpClientModule, CommonModule, LoaderComponent],
})
export class CartProductCardComponent {
  quantity = 0;
  private cartSubscription: Subscription;
  product: Product;
  @Input() productId: number;
  constructor(
    private productService: productService,
    private cartService: CartService
  ) {}
  

  ngOnInit(): void {
    this.quantity = 0;
    this.cartSubscription = this.cartService
      .getCartItems$()
      .subscribe((items) => {
        const item = items.find((item) => item.productId === this.productId);
        if (item) {
          this.quantity = item.quantity;
          this.productService
            .fetchProductById(item.productId)
            .subscribe((response) => (this.product = response));
        }
      });
  }

  incrementProductCount(productId: number) {
    this.cartService.incrementCountByProductId(productId);
  }

  decrementProductCount(productId: number) {
    if (this.cartService.findQuantityByProductId(productId) > 1)
      this.cartService.decrementCountByProductId(productId);
    else this.cartService.removeFromCart(productId);
  }
}
