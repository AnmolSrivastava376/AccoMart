import { AfterContentInit, Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { ActivatedRoute } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { LoaderComponent } from '../loader/loader.component';
import { CartService } from '../../services/cart.services';
import { productService } from '../../services/product.services';
import { BuyNowService } from '../../services/buy-now.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-detail-card',
  standalone: true,
  imports: [CommonModule, NavbarComponent, HttpClientModule, LoaderComponent],
  templateUrl: './product-detail-card.component.html',
  styleUrl: './product-detail-card.component.css',
})
export class ProductDetailCardComponent implements OnInit, AfterContentInit, OnChanges {
  @Input() product?: Product;
  productId: number;
  isLoading: boolean = false;
  displayText: string = 'ADD TO CART';
  constructor(
    private route: ActivatedRoute,
    private cartService: CartService,
    private productService: productService,
    private buyNowService: BuyNowService,
    private router : Router
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.productId = +params['productId'];
    });
  }
  ngOnChanges(changes: SimpleChanges): void {
      if(changes['product']){
        if(this.product && this.product.stock===0){
          this.displayText = 'OUT OF STOCK'
        }
      }
  }

  ngAfterContentInit(): void {
    const items = JSON.parse(localStorage.getItem('cartItems') || '');
    items.forEach((item: Product) => {
      if (item.productId === this.productId) {
        this.displayText = 'VIEW IN CART';
      }
    });
  }

  handleClick() {
    if (this.displayText === 'VIEW IN CART') {
      this.router.navigate(['/home/cart']);
    }
    if (this.product && this.product.stock > 0) {
      this.cartService.addToCart(this.productId);
      this.displayText = 'VIEW IN CART';
    }
  }

  handleBuyNowClick() {
    if(this.product && this.product.stock>0){
      this.productService
      .fetchProductById(this.productId)
      .subscribe((response: Product) => {
        this.buyNowService.item = {
          productId: response.productId,
          quantity: 1,
        };
        this.router.navigate(['/home/buy-product', this.productId]);
      });
    }
    else{
      alert("Product out of stock")
    }
  }
}
