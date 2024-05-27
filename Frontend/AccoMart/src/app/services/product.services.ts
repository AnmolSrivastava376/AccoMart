import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from '../interfaces/product';
import { CreateProduct } from '../interfaces/createProduct';

@Injectable({
  providedIn: 'root',
})
export class productService {
  constructor(private http: HttpClient) {}

  fetchProductByCategoryID(categoryId: number): Observable<Product[]> {
    return this.http.get<Product[]>(
      `http://localhost:5239/AdminDashboard/Products/CategoryId=${categoryId}?orderBy=price_dsc`
    );
  }

  fetchProductByCategoryName(categoryName: string): Observable<Product[]> {
    return this.http.get<Product[]>(
      `http://localhost:5239/AdminDashboard/Products/CategoryName=${categoryName}`
    );
  }

  fetchProductById(productId: number): Observable<Product> {
    return this.http.get<Product>(
      `http://localhost:5239/AdminDashboard/Product/${productId}`
    );
  }
  fetchProductByPageNo(
    categoryId: number,
    pageNo: number
  ): Observable<Product[]> {
    return this.http.get<Product[]>(
      `http://localhost:5239/AdminDashboard/ProductsByPageNo?id=${categoryId}&pageNo=${pageNo}`
    );
  }

  fetchProductByName(productName: string): Observable<Product[]> {
    return this.http.get<Product[]>(
      `http://localhost:5239/AdminDashboard/Products/SearchBy=${productName}`
    );
  }

  fetchAllProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(
      'http://localhost:5239/AdminDashboard/Products'
    );
  }

  editProductById(
    productId: number,
    updatedProduct: Product
  ): Observable<Product> {
    return this.http.put<Product>(
      `http://localhost:5239/AdminDashboard/Update/Product/${productId}`,
      updatedProduct
    );
  }

  addProduct(product: CreateProduct): Observable<any> {
    return this.http.post(
      'http://localhost:5239/AdminDashboard/Product/Create',
      product
    );
  }

  deleteProductById(productId: number): Observable<any> {
    return this.http.delete(
      `http://localhost:5239/AdminDashboard/Delete/Product/${productId}`
    );
  }
}
