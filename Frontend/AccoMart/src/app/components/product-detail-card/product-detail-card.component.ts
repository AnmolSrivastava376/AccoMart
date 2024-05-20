import { Component, Input, OnInit } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { LoaderComponent } from '../loader/loader.component';


@Component({
  selector: 'app-product-detail-card',
  standalone: true,
  imports: [CommonModule, NavbarComponent, HttpClientModule, LoaderComponent],

  templateUrl: './product-detail-card.component.html',
  styleUrl: './product-detail-card.component.css',
})
export class ProductDetailCardComponent implements OnInit {
  @Input() product?: Product;
  productId: number;
  isLoading: boolean=false;
  

  constructor(private route: ActivatedRoute  ) {}
  
  
  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.productId = +params['productId'];
      // this.fetchProductDetails(this.productId);
      
    });
  }

  // fetchProductDetails(productId:number): void{
  //   this.isLoading = true;
  //   this.productService.getProduct(productId).subscribe(
  //   (product: Product | undefined) => {
  //     this.product = product;
  //     this.isLoading = false;
  //   },
  //   (error: any) => {
  //     console.error('Error fetching product details:', error);
  //     this.isLoading = false;
  //   }
  // );
  // }
   
}

