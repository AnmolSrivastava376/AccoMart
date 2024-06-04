import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { createDeliveryService } from '../interfaces/createDeliveryService';
import { DeliveryService } from '../interfaces/deliveryService';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class deliveryService {
  constructor(private http: HttpClient) {}

  baseUrl = environment.serverUrl;

  getDeliveryServices(): Observable<DeliveryService[]> {
    return this.http.get<DeliveryService[]>(`${this.baseUrl}DeliveryServiceController/GetAllDeliveryServices`);
  }

  addDeliveryService(deliveryService: createDeliveryService): Observable<any> {
    return this.http.post(
      `${this.baseUrl}DeliveryServiceController/AddDeliveryService`,
      deliveryService
    );
  }

  editDeliveryService(
    deliveryService: createDeliveryService,
    id: number
  ): Observable<any> {
    return this.http.put(
      `${this.baseUrl}DeliveryServiceController/UpdateDeliveryService/${id}`,
      deliveryService
    );
  }

  deleteDeliveryService(id: number): Observable<any> {
    return this.http.delete(
      `${this.baseUrl}DeliveryServiceController/DeleteDeliveryService/${id}`
    );
  }

  getDeliveryDate(deliveryServiceId: number): Observable<number> {
    return this.http.get<number>(
      `${this.baseUrl}DeliveryServiceController/GetDeliveryDays/${deliveryServiceId}`
    );
  }
}
