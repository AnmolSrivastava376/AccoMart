import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ChartOrderItem } from '../interfaces/chartOrderItem';
import { ChartCategoryItem } from '../interfaces/chartCategoryItem';
import { ChartProductItem } from '../interfaces/chartProductItem';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ChartService {
  constructor(private http: HttpClient) {}

  baseUrl = environment.serverUrl + 'ChartController/';

  fetchDailyOrderQuantity(): Observable<ChartOrderItem[]> {
    return this.http.get<ChartOrderItem[]>(`${this.baseUrl}FetchDailyOrderQuantity`);
  }

  fetchCategoryWiseQuantity(): Observable<ChartCategoryItem[]> {
    return this.http.get<ChartCategoryItem[]>(`${this.baseUrl}FetchCategoryWiseQuantity`);
  }

  fetchProductWiseQuantity(): Observable<ChartProductItem[]> {
    return this.http.get<ChartProductItem[]>(`${this.baseUrl}FetchProductWiseQuantity`);
  }
}
