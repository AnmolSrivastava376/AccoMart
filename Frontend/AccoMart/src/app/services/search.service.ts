import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../interfaces/product';

@Injectable({
  providedIn: 'root',
})
export class searchService {
  constructor(private http: HttpClient) {}

  searchProductByprefix(prefix: string): Observable<Product[]> {
    return this.http.get<Product[]>(
      `http://localhost:5239/AdminDashboard/Products/SearchBy=${prefix}`
    );
  }
}