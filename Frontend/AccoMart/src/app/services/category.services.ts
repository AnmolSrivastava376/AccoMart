import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../interfaces/category';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  baseUrl: string
  constructor(private http: HttpClient) {
    this.baseUrl = environment.serverUrl;
  }

  fetchCategories(): Observable<Category[]> {
    const url = `${this.baseUrl}AdminDashboard/GetAllCategories`
    console.log(url);
    return this.http.get<Category[]>(url);
  }

  fetchCategoriesByAdmin(): Observable<Category[]> {
    const url = `${this.baseUrl}AdminDashboard/GetAllCategoriesAdmin`
    console.log(url);
    return this.http.get<Category[]>(url);
  }


  fetchCategorybyName(categoryName: string): Observable<Category> {
    return this.http.get<Category>(
      `${this.baseUrl}AdminDashboard/category/name/${categoryName}`
    );
  }

  fetchCategorybyId(categoryId: number): Observable<Category> {
    return this.http.get<Category>(
      `${this.baseUrl}AdminDashboard/category/${categoryId}`
    );
  }

  addCategory(categoryName: string): Observable<any> {
    return this.http.post(
      `${this.baseUrl}AdminDashboard/Category/Create`,
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
