import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import axios from "axios";

@Injectable({
  providedIn: 'root',
})
export class HttpService {
  apiUrl = 'http://localhost:5280';
  http = inject(HttpClient);
  constructor() {}

  fetchCategories() {
    return axios.post<Category[]>('http://localhost:5239/AdminDashboard/GetAllCategories');
  }
}
