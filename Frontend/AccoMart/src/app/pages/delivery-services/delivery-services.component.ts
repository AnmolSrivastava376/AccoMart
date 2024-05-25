import { Component } from '@angular/core';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { deliveryService } from '../../services/delivery.service';
import { DeliveryService } from '../../interfaces/deliveryService';
import { CommonModule } from '@angular/common';
import { createDeliveryService } from '../../interfaces/createDeliveryService';
import { FormsModule } from '@angular/forms'; // Import FormsModule
import {HttpClientModule } from '@angular/common/http';
import {ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-delivery-services',
  standalone: true,
  providers:[deliveryService],
  imports: [SidebarComponent,NavbarComponent,CommonModule,FormsModule,HttpClientModule],
  templateUrl: './delivery-services.component.html',
  styleUrl: './delivery-services.component.css'
})
export class DeliveryServicesComponent {


  constructor(private deliveryService: deliveryService,private toastr : ToastrService) {}
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
    this.deliveryService.getDeliveryServices().subscribe(
      (response: any) => {
        if (response.isSuccess) {
          this.deliveryServicesList = response.response as DeliveryService[];
        }
        else {
          console.error('Error fetching delivery services:', response.message);

        }
      },
      error => {
        console.error('Error fetching delivery services:', error);
      }
    );
  }

  deleteDeliveryService(id: number) {
    if(confirm("Are you sure you want to delete this Service?")) {
      this.deliveryService.deleteDeliveryService(id).subscribe(
        () => {
          this.fetchDeliveryServices();
        },
        error => {
          console.error('Error deleting delivery service:', error);
        }
      );
    }
  }

  editDeliveryService(deliveryService: createDeliveryService) {
    console.log(deliveryService, this.editServiceId);
    this.deliveryService.editDeliveryService(deliveryService, this.editServiceId).subscribe(
      (response: any) => {
        if (response.isSuccess) {
          this.toastr.success('Delivery Service successfully updated');
          this.openEditServicePopup = false;
          this.fetchDeliveryServices();
        } else {
          this.toastr.error("Error editing delivery service:");
        }
      },
      error => {
        console.error('Error editing delivery service:', error);
      }
    );
  }

  createDeliveryService(newDeliveryService: createDeliveryService) {
    this.deliveryService.addDeliveryService(newDeliveryService).subscribe(
      (response: any) => {
        if (response.isSuccess) {
          this.toastr.success("Delivery service created");
          this.openAddServicePopup = false;
          this.serviceToAdd.deliveryDays = 0;
          this.serviceToAdd.imageUrl = '';
          this.serviceToAdd.serviceName = '';
          this.serviceToAdd.price = 0;

          this.fetchDeliveryServices();
        } else {
          this.toastr.error("Error creating delivery service:");
        }
      },
      error => {
        this.toastr.error("Error creating delivery service:",error);
      }
    );
  }

  openAddPopup() {
    this.openAddServicePopup = true;
  }

  openEditPopup(Id:number,service:createDeliveryService) {
    this.editServiceId = Id;
    this.openEditServicePopup = true;
    this.serviceToEdit = {...service};
  }

  closeAddPopup() {
    this.openAddServicePopup = false;
  }

  closeEditPopup() {
    this.openEditServicePopup = false;
  }
}
