import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../interfaces/category';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  constructor(private http: HttpClient) {}

  fetchCategories(): Observable<Category[]> {
    return this.http.get<Category[]>('http://localhost:5239/AdminDashboard/GetAllCategories');
  }

  addCategory(categoryName: string): Observable<any> {
    return this.http.post(`http://localhost:5239/AdminDashboard/Category/Create`, { name: categoryName });
}
  deleteCategory(categoryId: number): Observable<any> {
    return this.http.delete(`http://localhost:5239/AdminDashboard/Delete/Category/${categoryId}`);
  }

  editCategory(categoryId: number, newName: string): Observable<any> {
    return this.http.put(`http://localhost:5239/AdminDashboard/Update/Category?Id=${categoryId}&NewCategoryName=${newName}`, {});
  }
}
