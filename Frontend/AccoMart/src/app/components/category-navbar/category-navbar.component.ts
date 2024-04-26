import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Category } from '../../interfaces/category';

@Component({
  selector: 'app-category-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './category-navbar.component.html',
  styleUrl: './category-navbar.component.css'
})
export class CategoryNavbarComponent {
  @Input() categories?: Category[];
  @Output() categorySelected = new EventEmitter<number>();

  onCategoryClick(categoryId: number) {
    this.categorySelected.emit(categoryId);
  }
}
