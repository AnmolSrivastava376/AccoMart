import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import axios from "axios";

@Injectable({
  providedIn: 'root',
})
export class InvoiceService {

  http = inject(HttpClient);
  constructor() {}

  getInvoice() {
    return this.http.get<any>('http://localhost:5239/GetInvoice/34', {
    });
  }
}
