import { Component, Input } from '@angular/core';
import { Product } from '../../interfaces/product';
import { productService } from '../../services/product.services';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.css'
})
export class ProductDetailComponent {
  product: Product | null | undefined ;
  productId: number = 1; // Set the productId to the desired product ID

  constructor(
    private route: ActivatedRoute,
    private productService: productService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.productId = params['id'];
        this.fetchProductDetails();
      }
    });
  }

  fetchProductDetails(): void {
    this.productService.fetchProductById(this.productId).then(response => {
      console.log(response.data);
      this.product = response.data;
      console.log('Fetched product:', this.product);
    }).catch(error => {
      console.error('Error fetching product:', error);
    });
  }

}
