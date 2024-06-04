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

  baseUrl = environment.serverUrl+ 'AdminDashboard/';

  fetchCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}GetAllCategories`);
  }

  fetchCategorybyName(categoryName: string): Observable<Category> {
    return this.http.get<Category>(
      `${this.baseUrl}${categoryName}`
    );
  }

  fetchCategorybyId(categoryId: number): Observable<Category> {
    return this.http.get<Category>(
      `${this.baseUrl}${categoryId}`
    );
  }

  addCategory(categoryName: string): Observable<any> {
    return this.http.post(
      `${this.baseUrl}Create`,
      { name: categoryName }
    );
  }

  deleteCategory(categoryId: number): Observable<any> {
    return this.http.delete(
      `${this.baseUrl}Delete/Category/${categoryId}`
    );
  }

  editCategory(categoryId: number, newName: string): Observable<any> {
    return this.http.put(
      `${this.baseUrl}Update/Category?Id=${categoryId}&NewCategoryName=${newName}`,
      {}
    );
  }
}
