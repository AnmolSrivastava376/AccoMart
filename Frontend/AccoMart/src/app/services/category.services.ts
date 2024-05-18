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
