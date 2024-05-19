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

  addCategory(categororyName:string)
  {
    return axios.post(`http://localhost:5239/AdminDashboard/Category/Create?categoryName=${categororyName}`);
  }

  

  deleteCategory(categoryId: number)
  {
    return axios.delete(`http://localhost:5239/AdminDashboard/Delete/Category/${categoryId}`)
  }

  editCategory(categoryId:number,newName:string)
  {
    return axios.put(`http://localhost:5239/AdminDashboard/Update/Category?Id=${categoryId}&NewCategoryName=${newName}`)
  }
}
