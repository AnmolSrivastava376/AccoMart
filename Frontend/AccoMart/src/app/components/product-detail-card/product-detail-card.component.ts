import { AfterContentInit, Component, Input, OnInit } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { ActivatedRoute } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { LoaderComponent } from '../loader/loader.component';
import { CartService } from '../../services/cart.services';
import { productService } from '../../services/product.services';
import { BuyNowService } from '../../services/buy-now.service';

@Component({
  selector: 'app-product-detail-card',
  standalone: true,
  imports: [CommonModule, NavbarComponent, HttpClientModule, LoaderComponent],
  templateUrl: './product-detail-card.component.html',
  styleUrl: './product-detail-card.component.css',
})
export class ProductDetailCardComponent implements OnInit, AfterContentInit{
  @Input() product?: Product;
  productId: number;
  isLoading: boolean=false;
  displayText:string = "ADD TO CART"
  constructor(private route: ActivatedRoute, private cartService: CartService, private productService: productService, private buyNowService: BuyNowService) {}
  
  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.productId = +params['productId'];
    });
  }

  ngAfterContentInit(): void {
    const items = JSON.parse(localStorage.getItem('cartItems')||'');
    items.forEach((item:Product) => {
      if(item.productId===this.productId){
        this.displayText = "VIEW IN CART"
      }
    });
  }

  handleClick(){
    if(this.displayText === "VIEW IN CART"){
      window.location.href = 'home/cart'
    }
    this.cartService.addToCart(this.productId);
    this.displayText = "VIEW IN CART"
  }
  handleBuyNowClick(){
    this.productService.fetchProductById(this.productId).subscribe(
      (response: Product)=>{
        this.buyNowService.item = {"productId": response.productId, "quantity":1}
        window.location.href = `/home/buy-product/${this.productId}`
      }
    )
  }
}

