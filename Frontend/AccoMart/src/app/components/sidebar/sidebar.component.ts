import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  constructor(private router: Router) { }

  showProducts() {
    this.router.navigate(['/admin/products']);
    
  }

  showCategories() {
    this.router.navigate(['/admin/categories']);
  }

  showDeliveryServices(){
    this.router.navigate(['/admin/delivery'])
  }


}
