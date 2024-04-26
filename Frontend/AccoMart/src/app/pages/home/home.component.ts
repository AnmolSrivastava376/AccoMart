import { Component, OnInit } from '@angular/core';
import { CategoryNavbarComponent } from '../../components/category-navbar/category-navbar.component';
import axios from 'axios';
import { Category } from '../../interfaces/category';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  imports: [CategoryNavbarComponent],
})
export class HomeComponent implements OnInit {
  categories: Category[] = [];
  ngOnInit(): void {
    this.fetchCategories();
  }
  fetchCategories() {
    axios.get<Category[]>('http://localhost:5239/AdminDashboard')
      .then((response) => {
        console.log(response.data)
        this.categories = response.data;
      })
      .catch((error) => {
        console.error('Error fetching categories:', error);
      });
  }
}
