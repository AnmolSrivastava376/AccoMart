import { Injectable, inject } from '@angular/core';
import axios from 'axios';
import { Category } from '../interfaces/category';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  http = inject(HttpClient);
  fetchCategories() {
    return this.http.get<Category[]>('http://localhost:5239/AdminDashboard/GetAllCategories');
  }
}
