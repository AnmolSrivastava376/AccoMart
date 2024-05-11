import { HttpClientModule } from '@angular/common/http';
import { Component, Input } from '@angular/core';
import { productService } from '../../services/product.services';
import { Product } from '../../interfaces/product';

@Component({
  selector: 'app-cart-product-card',
  standalone: true,
  imports: [HttpClientModule],
  providers: [productService],
  templateUrl: './cart-product-card.component.html',
  styleUrl: './cart-product-card.component.css'
})
export class CartProductCardComponent {
  constructor(private productService : productService) {}
  product: Product={
    productId: 0,
    productName: '',
    productDesc: '',
    productPrice: 0,
    productImageUrl: '',
    categoryId: 0
  };
  @Input() productId: number;
  ngOnInit(): void {
    this.productService.fetchProductById(this.productId)
      .then((response) => {
        this.product = response.data;
        console.log(this.product);
      })
      .catch((error) => {
        console.error('Error fetching product:', error);
      })
  }
}
