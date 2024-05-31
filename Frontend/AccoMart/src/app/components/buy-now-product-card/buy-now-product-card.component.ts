import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Product } from '../../interfaces/product';
import { productService } from '../../services/product.services';
import { LoaderComponent } from '../loader/loader.component';
import { BuyNowService } from '../../services/buy-now.service';
import { cartItem } from '../../interfaces/cartItem';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-buy-now-product-card',
  standalone: true,
  imports: [CommonModule, LoaderComponent],
  templateUrl: './buy-now-product-card.component.html',
  styleUrl: './buy-now-product-card.component.css',
  providers: [productService, BuyNowService],
})
export class BuyNowProductCardComponent implements OnInit {
  @Input() productId: number;
  product: Product | null;
  @Input() productItem: cartItem;
  @Output() outputCartItem: EventEmitter<cartItem> =
    new EventEmitter<cartItem>();
  constructor(
    private productService: productService,
    private buyNowService: BuyNowService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.productService
      .fetchProductById(this.productId)
      .subscribe((response) => {
        this.product = response;
      });
  }

  incrementProductCount(stock: number) {
    if (this.productItem.quantity < stock) {
      this.productItem.quantity++;
      this.buyNowService.item = this.productItem;
      this.outputCartItem.emit(this.productItem);
    }
    else{
      this.toastr.info("Product Quantity out of stock")
    }
  }

  decrementProductCount() {
    if (this.productItem.quantity > 1) {
      this.productItem.quantity--;
      this.buyNowService.item = this.productItem;
      this.outputCartItem.emit(this.productItem);
    } else {
      this.buyNowService.removeItem();
      window.location.href = `home`;
    }
  }
}
