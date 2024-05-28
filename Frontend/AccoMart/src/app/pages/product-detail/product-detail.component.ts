import { Component, OnInit } from '@angular/core';
import { Product } from '../../interfaces/product';
import { ActivatedRoute } from '@angular/router';
import { productService } from '../../services/product.services';
import { ProductDetailCardComponent } from '../../components/product-detail-card/product-detail-card.component';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [ProductDetailCardComponent, HttpClientModule],
  providers: [productService],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.css'],
})
export class ProductDetailComponent implements OnInit {
  product?: Product;
  productId: number = 1;
  constructor(
    private route: ActivatedRoute,
    private productService: productService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const productIdFromRoute = params['productId'];
      this.productId = +productIdFromRoute;
      this.fetchProductDetails();
    });
  }

  fetchProductDetails(): void {
    this.productService.fetchProductById(this.productId).subscribe({
      next: (response) => {
        this.product = response;
      },
      error: (error) => {
        console.error('Error fetching product:', error);
      },
    });
  }
}
