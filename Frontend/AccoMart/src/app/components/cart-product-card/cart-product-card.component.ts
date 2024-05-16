import { HttpClientModule } from '@angular/common/http';
import { Component, Input } from '@angular/core';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';
import { CartService } from '../../services/cart.services';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cart-product-card',
  standalone: true,
  imports: [HttpClientModule,CommonModule],
  providers: [productService],
  templateUrl: './cart-product-card.component.html',
  styleUrl: './cart-product-card.component.css'
})
export class CartProductCardComponent {
  quantity=0
  private cartSubscription: Subscription;
  constructor(private productService : productService, private cartService: CartService) {}
  product: Product
  @Input() productId: number;
  ngOnInit(): void {
    this.quantity = this.cartService.findQuantityByProductId(this.productId)
    this.cartSubscription = this.cartService.getCartItems$().subscribe(
      item=>{
        this.quantity = item.find(i=>i.productId === this.productId)?.quantity || 0
      }
    )
    this.productService.fetchProductById(this.productId)
      .then((response) => {
        this.product = response.data;
      })
      .catch((error) => {
        console.error('Error fetching product:', error);
      })
  }
  incrementProductCount(productId: number){
    this.cartService.incrementCountByProductId(productId);
  }
  decrementProductCount(productId: number){
    if(this.cartService.findQuantityByProductId(productId)>1)
      this.cartService.decrementCountByProductId(productId);
    else
      this.cartService.removeFromCart(productId);
  }
}
