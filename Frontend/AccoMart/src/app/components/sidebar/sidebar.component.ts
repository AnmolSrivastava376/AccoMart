import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [MatIcon, HttpClientModule, CommonModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css',
  providers: []
})
export class SidebarComponent implements OnInit {
  currentRoute: string = '';
  constructor(private router: Router, private activatedRoute: ActivatedRoute) {}

  ngOnInit(): void {
    this.activatedRoute.url.subscribe((urlSegments) => {
      this.currentRoute = urlSegments[urlSegments.length - 1].path;
    });
  }

  showProducts() {
    this.router.navigate(['/admin/products'])
  }

  showCategories() {
    this.router.navigate(['/admin/categories'])


  }

  showDeliveryServices() {
    this.router.navigate(['/admin/delivery'])
  }

  showDashboard() {
    this.router.navigate(['/admin'])
  }
}