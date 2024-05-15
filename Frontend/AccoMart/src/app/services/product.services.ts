import { Injectable, inject } from '@angular/core';
import axios from 'axios';
import { Product } from '../interfaces/product';
import { HttpClient } from '@angular/common/http';
import { CreateProduct } from '../interfaces/createProduct';


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

  editProductById(productId: number, updatedProduct: Product) {
    return axios.put<Product>(`http://localhost:5239/AdminDashboard/Update/Product/${productId}`, updatedProduct);
  }
  
  addProduct(product:CreateProduct)
  {
    return axios.post('http://localhost:5239/AdminDashboard/Product/Create',product);
  }
  deleteProductById(productId:number)
  {
    return axios.delete(`http://localhost:5239/AdminDashboard/Delete/Product/${productId}`);
  }

  
  }



