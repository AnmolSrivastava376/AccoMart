import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../interfaces/category';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  constructor(private http: HttpClient) {}

  baseUrl = environment.serverUrl;

  fetchCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}AdminDashboard/GetAllCategories`);
  }

  fetchCategorybyName(categoryName: string): Observable<Category> {
    return this.http.get<Category>(
      `${this.baseUrl}AdminDashboard/${categoryName}`
    );
  }

  fetchCategorybyId(categoryId: number): Observable<Category> {
    return this.http.get<Category>(
      `${this.baseUrl}AdminDashboard/${categoryId}`
    );
  }

  addCategory(categoryName: string): Observable<any> {
    return this.http.post(
      `${this.baseUrl}AdminDashboard/Create`,
      { name: categoryName }
    );
  }

  deleteCategory(categoryId: number): Observable<any> {
    return this.http.delete(
      `${this.baseUrl}AdminDashboard/Delete/Category/${categoryId}`
    );
  }

  editCategory(categoryId: number, newName: string): Observable<any> {
    return this.http.put(
      `${this.baseUrl}AdminDashboard/Update/Category?Id=${categoryId}&NewCategoryName=${newName}`,
      {}
    );
  }
}
