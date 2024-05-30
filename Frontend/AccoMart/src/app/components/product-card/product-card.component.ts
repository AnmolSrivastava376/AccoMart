import { Component, EventEmitter, Input, Output, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { GridDisplayCardComponent } from '../grid-display-card/grid-display-card.component';
import { ScrollDisplayCardComponent } from '../scroll-display-card/scroll-display-card.component';
import { ProductScrollDisplayCardComponent } from '../product-scroll-display-card/product-scroll-display-card.component';
import { HttpClientModule } from '@angular/common/http';
import { ChartService } from '../../services/chart.service';
import { productService } from '../../services/product.services';
import { ChartProductItem } from '../../interfaces/chartProductItem';
import { MoreProducts } from '../../interfaces/moreProducts';
import { Category } from '../../interfaces/category';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [
    CommonModule,
    GridDisplayCardComponent,
    ScrollDisplayCardComponent,
    ProductScrollDisplayCardComponent,
    HttpClientModule,
  ],
  providers: [ChartService, productService],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.css',
})
export class ProductCardComponent implements OnInit,OnChanges {
  trendingProducts: Product[] = [];
  orderedProducts: ChartProductItem[];
  moreProducts: MoreProducts[]=[];
  @Input() products?: Product[];
  @Input() categoryName?: string;
  @Input() categories: Category[]
  @Input() filteredProducts?: Product[];
  @Output() fetchNextPage: EventEmitter<boolean> = new EventEmitter<boolean>();
  constructor(
    private chartService: ChartService,
    private productService: productService
  ) {}
  ngOnChanges(changes: SimpleChanges): void {
      if(changes['categories']){
        this.fetchBrowseMoreProducts();
      }
  }
  ngOnInit() {
    this.chartService.fetchProductWiseQuantity().subscribe((data) => {
      this.orderedProducts = data
        .sort((a, b) => b.quantity - a.quantity)
        .slice(0, 7);
      for (let i = this.orderedProducts.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [this.orderedProducts[i], this.orderedProducts[j]] = [
          this.orderedProducts[j],
          this.orderedProducts[i],
        ];
      }
      this.orderedProducts.forEach((product) =>
        this.productService
          .fetchProductById(product.productId)
          .subscribe((resp) => {
            if (resp !== undefined) {
              this.trendingProducts = [...this.trendingProducts, resp];
            }
          })
      );
    });
  }
  fetchBrowseMoreProducts(){
    this.categories.forEach((category)=>{
      this.productService.fetchProductByPageNo(category.categoryId, 1).subscribe({
        next: (response)=>{
          this.moreProducts.push({categoryId: category.categoryId, items: response, pageNo: 1})
        }
      })
    })
  }
  handleClick(productId: number) {
    window.location.href = `home/productdetail/${productId}`;
  }

  handleEmitter() {
    this.fetchNextPage.emit(true);
  }
  handleEmitterCategoryWise(categoryId: number){
    this.moreProducts.forEach((item)=>{
      if(item.categoryId === categoryId){
        this.productService.fetchProductByPageNo(categoryId, item.pageNo+1).subscribe({
          next: (response)=>{
            if(response.length>0){
              item.items = item.items.concat(response)
            }
          },
          complete: ()=>{
            item.pageNo++;
          }
        })
      }
    })
  }
}
