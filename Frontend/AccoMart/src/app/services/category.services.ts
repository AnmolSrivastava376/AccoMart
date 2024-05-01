import { Injectable } from '@angular/core';
import axios from 'axios';
import { Category } from '../interfaces/category';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  fetchCategories() {
    return axios.get<Category[]>('http://localhost:5239/AdminDashboard/GetAllCategories');
  }
}
