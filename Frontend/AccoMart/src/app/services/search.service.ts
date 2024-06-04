import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../interfaces/product';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class searchService {
  constructor(private http: HttpClient) {}

  baseUrl = environment.serverUrl + 'AdminDashboard/Products/';

  searchProductByprefix(prefix: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.baseUrl}SearchBy=${prefix}`);
  }
}
