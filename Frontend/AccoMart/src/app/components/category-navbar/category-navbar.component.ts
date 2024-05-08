import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Category } from '../../interfaces/category';
import {MatIconModule} from '@angular/material/icon';

@Component({
  selector: 'app-category-navbar',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './category-navbar.component.html',
  styleUrl: './category-navbar.component.css'
})
export class CategoryNavbarComponent {
  @Input() categories?: Category[];
  @Output() categorySelected = new EventEmitter<number>();
  @Output() categoryIndex = new EventEmitter<number>();
  isCategoryNavbarActive: boolean = false;
  isFirstLoad: boolean = true;
  shouldAnimate:boolean = false;

  onCategoryClick(categoryId: number, index: number) {
    this.categorySelected.emit(categoryId);
    this.categoryIndex.emit(index);
  }
  toogleCategoryButtonClick(){
    this.isCategoryNavbarActive=!this.isCategoryNavbarActive;
    if(this.isFirstLoad){
      this.isFirstLoad = !this.isFirstLoad;
    }
    if(!this.isFirstLoad){
      this.shouldAnimate=true;
    }
  }
}
