import { HttpClientModule } from '@angular/common/http';
import { Component, Input } from '@angular/core';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [MatIcon,HttpClientModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  constructor() { }

  showProducts() {
    window.location.href = '/admin/products';
  }

  showCategories() {
    window.location.href = '/admin/categories';
  }

  showDeliveryServices(){
    window.location.href = '/admin/delivery';
  }
  showDashboard(){
    window.location.href = '/admin';
  }
}
