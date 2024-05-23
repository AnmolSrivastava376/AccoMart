import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import axios from "axios";
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class invoiceService {

  http = inject(HttpClient);
  constructor() {}

  getInvoice(orderId : number) :Observable<Blob> {
    return this.http.get(`http://localhost:5239/GetInvoice/${orderId}`, {
      responseType: 'blob'
    });
  }

}


