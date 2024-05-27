import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ChartOrderItem } from '../interfaces/chartOrderItem';
import { ChartCategoryItem } from '../interfaces/chartCategoryItem';
import { ChartProductItem } from '../interfaces/chartProductItem';

@Injectable({
  providedIn: 'root',
})
export class ChartService {
  constructor(private http: HttpClient) {}
  fetchDailyOrderQuantity(): Observable<ChartOrderItem[]> {
    return this.http.get<ChartOrderItem[]>(
      'http://localhost:5239/ChartController/FetchDailyOrderQuantity'
    );
  }
  fetchCategoryWiseQuantity(): Observable<ChartCategoryItem[]> {
    return this.http.get<ChartCategoryItem[]>(
      'http://localhost:5239/ChartController/FetchCategoryWiseQuantity'
    );
  }
  fetchProductWiseQuantity(): Observable<ChartProductItem[]> {
    return this.http.get<ChartProductItem[]>(
      'http://localhost:5239/ChartController/FetchProductWiseQuantity'
    );
  }
}
