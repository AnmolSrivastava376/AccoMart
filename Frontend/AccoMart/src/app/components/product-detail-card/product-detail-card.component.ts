import { Component, Input, OnInit } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-product-detail-card',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './product-detail-card.component.html',
  styleUrl: './product-detail-card.component.css'
})
export class ProductDetailCardComponent implements OnInit {
   @Input() product?:Product;
   productId: number

   constructor(private route : ActivatedRoute) {


   }
  ngOnInit(): void {

    this.route.params.subscribe(params => {
      this.productId = +params['productId'];
    });
  }
}
