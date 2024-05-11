import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.css'
})
export class AdminProductsComponent implements OnInit {
  constructor(private router: Router ,private http:HttpClient) { }

  products: any[] = [];


  ngOnInit(): void {
    this.fetchProducts();
  }

  fetchProducts() {
    this.http.get<any[]>('YOUR_BACKEND_API_URL/products')
      .subscribe(products => {
        this.products = products;
      });
  }

  showProducts() {
    this.router.navigate(['/admin/products']);
  }

  showCategories() {
    this.router.navigate(['/admin/categories']);
  }
}
