import { Component, OnInit } from '@angular/core';
import { CategoryNavbarComponent } from '../../components/category-navbar/category-navbar.component';
import axios from 'axios';
import { Category } from '../../interfaces/category';
import { CategoryService } from '../../services/category.services';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [CategoryNavbarComponent],
})
export class HomeComponent implements OnInit {
  categories: Category[] = [];
  constructor(private categoryService: CategoryService) {}
  
  ngOnInit(): void {
    this.categoryService.fetchCategories()
      .then((response) => {
        this.categories = response.data;
      })
      .catch((error) => {
        console.error('Error fetching categories:', error);
      });
  }
}
