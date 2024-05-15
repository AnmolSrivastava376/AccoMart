import { Injectable, inject } from '@angular/core';
import axios from 'axios';
import { Product } from '../interfaces/product';
import { HttpClient } from '@angular/common/http';


@Injectable({
  providedIn:'root'
})
export class productService {
  http = inject(HttpClient);
  fetchProductByCategoryID(categoryId:Number) {
    return axios.get<Product[]>(`http://localhost:5239/AdminDashboard/Products/CategoryId=${categoryId}?orderBy=price_dsc`);
  }
  fetchProductById(productId: number) {
    return axios.get<Product>(`http://localhost:5239/AdminDashboard/Product/${productId}`);
  }

  fetchAllProducts(){
    return axios.get<Product[]>('http://localhost:5239/AdminDashboard/Products');
  }

  
  }



