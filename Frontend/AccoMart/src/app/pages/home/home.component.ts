import { Component, OnInit } from '@angular/core';
import { CategoryNavbarComponent } from '../../components/category-navbar/category-navbar.component';
import { Category } from '../../interfaces/category';
import { CategoryService } from '../../services/category.services';
import { ProductCardComponent } from '../../components/product-card/product-card.component';
import { Product } from '../../interfaces/product';
import { productService } from '../../services/product.services';
import { NavbarComponent } from '../../components/navbar/navbar.component';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [CategoryNavbarComponent, ProductCardComponent, NavbarComponent],
})
export class HomeComponent implements OnInit {
  categories: Category[] = [];
  products: Product[]=[];
  activeCategory: Number=-1;
  constructor(private categoryService: CategoryService, private productService: productService) {}
  
  ngOnInit(): void {
    this.categoryService.fetchCategories()
      .then((response) => {
        this.categories = response.data;
        this.activeCategory=this.categories[0].categoryId;
      })
      .catch((error) => {
        console.error('Error fetching categories:', error);
      }).then(()=>{
        if(this.activeCategory!=-1){
          this.productService.fetchProductByCategoryID(this.activeCategory).then((response)=>{
            this.products = response.data;
          }).catch((error)=>{
            console.error('Error fetching categories:', error);
          })
        }
      })
  }
  onCategorySelected(categoryId: number){
    this.activeCategory = categoryId;
    this.productService.fetchProductByCategoryID(this.activeCategory).then((response)=>{
      this.products = response.data;
    }).catch((error)=>{
      console.error('Error fetching categories:', error);
    })
  }
}
