import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../interfaces/product';
import { CreateProduct } from '../interfaces/createProduct';
import { environment } from '../../environments/environment';
@Injectable({
  providedIn: 'root',
})
export class productService {
  constructor(private http: HttpClient) {}

  baseUrl = environment.serverUrl

  fetchProductByCategoryID(categoryId: number): Observable<Product[]> {
    return this.http.get<Product[]>(
      `${this.baseUrl}AdminDashboard/Products/CategoryId=${categoryId}?orderBy=price_dsc`
    );
  }

  fetchProductByCategoryName(categoryName: string): Observable<Product[]> {
    return this.http.get<Product[]>(
      `${this.baseUrl}AdminDashboard/Products/CategoryName=${categoryName}`
    );
  }

  fetchProductById(productId: number): Observable<Product> {
    return this.http.get<Product>(
      `${this.baseUrl}AdminDashboard/Product/${productId}`
    );
  }
  fetchAllProductsPagewise(pageNo: number): Observable<Product[]> {
    return this.http.get<Product[]>(
      `${this.baseUrl}AdminDashboard/GetAllProductsPagewise?pageNo=${pageNo}`
    );
  }
  fetchProductByPageNo(categoryId: number, pageNo: number): Observable<Product[]> {
    return this.http.get<Product[]>(
      `${this.baseUrl}AdminDashboard/ProductsByPageNo?id=${categoryId}&pageNo=${pageNo}`
    );
  }

  fetchProductByName(productName: string): Observable<Product[]> {
    return this.http.get<Product[]>(
      `${this.baseUrl}AdminDashboard/Products/SearchBy=${productName}`
    );
  }

  fetchAllProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(
      `${this.baseUrl}AdminDashboard/Products`
    );
  }

  editProductById(
    productId: number,
    updatedProduct: Product
  ): Observable<Product> {
    return this.http.put<Product>(
      `${this.baseUrl}AdminDashboard/Update/Product/${productId}`,
      updatedProduct
    );
  }

  addProduct(product: CreateProduct): Observable<any> {
    return this.http.post(
      `${this.baseUrl}AdminDashboard/Product/Create`,
      product
    );
  }

  deleteProductById(productId: number): Observable<any> {
    return this.http.delete(
      `${this.baseUrl}AdminDashboard/Delete/Product/${productId}`
    );
  }
}
