import { Component } from '@angular/core';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { deliveryService } from '../../services/delivery.service';
import { DeliveryService } from '../../interfaces/deliveryService';
import { CommonModule } from '@angular/common';
import { createDeliveryService } from '../../interfaces/createDeliveryService';
import { FormsModule } from '@angular/forms'; // Import FormsModule
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-delivery-services',
  standalone: true,
  imports: [SidebarComponent,NavbarComponent,CommonModule,FormsModule,HttpClientModule],
  providers : [deliveryService],
  templateUrl: './delivery-services.component.html',
  styleUrl: './delivery-services.component.css'
})
export class DeliveryServicesComponent {

  constructor(private deliveryService: deliveryService) {}
  deliveryServicesList: DeliveryService[] = [];
  openAddServicePopup:boolean = false;
  openEditServicePopup:boolean = false;
  editServiceId:number;

  serviceToAdd: createDeliveryService = {
    imageUrl: '',
    serviceName: '',
    price: 0,
    deliveryDays: 0
  };

  serviceToEdit: createDeliveryService = {
    imageUrl: '',
    serviceName: '',
    price: 0,
    deliveryDays: 0
  };

  ngOnInit(): void {
    this.fetchDeliveryServices();
  }
  fetchDeliveryServices() {
    this.deliveryService.getDeliveryServices()
      .subscribe(
        (response) => {
          this.deliveryServicesList = response;
        },
        (error) => {
          console.error('Error fetching delivery services:', error);
        }
      );
  }

  deleteDeliveryService(id: number) {
    if (confirm("Are you sure you want to delete this Service?")) {
      this.deliveryService.deleteDeliveryService(id)
        .subscribe(
          () => {
            this.fetchDeliveryServices();
          },
          (error) => {
            console.error('Error deleting delivery service:', error);
          }
        );
    }
  }

  editDeliveryService(deliveryService: createDeliveryService) {
    this.deliveryService.editDeliveryService(deliveryService, this.editServiceId)
      .subscribe(
        () => {
          console.log("Delivery Service successfully updated");
          this.openAddServicePopup = false;
          this.fetchDeliveryServices();
        },
        (error) => {
          console.error('Error editing delivery service:', error);
        }
      );
  }

  createDeliveryService(newDeliveryService: createDeliveryService) {
    this.deliveryService.addDeliveryService(newDeliveryService)
      .subscribe(
        () => {
          // After successful creation, fetch the updated list of delivery services
          this.openAddServicePopup = false;
          this.fetchDeliveryServices();
        },
        (error) => {
          console.error('Error creating delivery service:', error);
        }
      );
  }


  openAddPopup()
  {
    this.openAddServicePopup = true;
  }

  openEditPopup(Id:number,service:createDeliveryService)
  {
    this.editServiceId = Id;
    this.openAddServicePopup = true;
    this.serviceToEdit = {...service};

  }


  closeAddPopup()
  {
    this.openAddServicePopup = false;
  }

  closeEditPopup()
  {
    this.openAddServicePopup = false;
  }

}


