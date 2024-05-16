import { Component } from '@angular/core';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { deliveryServices } from '../../services/delivery.service';
import { DeliveryService } from '../../interfaces/deliveryService';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-delivery-services',
  standalone: true,
  imports: [SidebarComponent,NavbarComponent,CommonModule],
  templateUrl: './delivery-services.component.html',
  styleUrl: './delivery-services.component.css'
})
export class DeliveryServicesComponent {
  constructor(private deliveryService: deliveryServices) {}
  deliveryServicesList: DeliveryService[] = [];

  ngOnInit(): void {
    this.fetchDeliveryServices();
  }
  fetchDeliveryServices() {
    this.deliveryService.getDeliveryServices().then(response => {
      this.deliveryServicesList = response.data;
    }).catch(error => {
      console.error('Error fetching delivery services:', error);
    });
  }

  deleteDeliveryService(id: number) {
    this.deliveryService.deleteDeliveryService(id).then(() => {
      // After successful deletion, fetch the updated list of delivery services
      this.fetchDeliveryServices();
    }).catch(error => {
      console.error('Error deleting delivery service:', error);
    });
  }

}
