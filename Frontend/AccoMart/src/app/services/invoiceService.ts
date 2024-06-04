import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class InvoiceService {
  constructor(private http: HttpClient) {}

  baseUrl = environment.serverUrl;

  getInvoice(orderId: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}GetInvoice/${orderId}`, {
      responseType: 'blob',
    });
  }
}
