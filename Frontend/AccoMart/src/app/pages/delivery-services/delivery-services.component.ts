import { Component } from '@angular/core';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { deliveryService } from '../../services/delivery.service';
import { DeliveryService } from '../../interfaces/deliveryService';
import { CommonModule } from '@angular/common';
import { createDeliveryService } from '../../interfaces/createDeliveryService';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-delivery-services',
  standalone: true,
  providers: [deliveryService],
  imports: [
    SidebarComponent,
    NavbarComponent,
    CommonModule,
    FormsModule,
    HttpClientModule,
  ],
  templateUrl: './delivery-services.component.html',
  styleUrl: './delivery-services.component.css',
})
export class DeliveryServicesComponent {
  deliveryServicesList: DeliveryService[] = [];
  openAddServicePopup: boolean = false;
  openEditServicePopup: boolean = false;
  editServiceId: number;
  isLoading: boolean = true;
  serviceToAdd: createDeliveryService = {
    imageUrl: '',
    serviceName: '',
    price: 0,
    deliveryDays: 0,
  };
  serviceToEdit: createDeliveryService = {
    imageUrl: '',
    serviceName: '',
    price: 0,
    deliveryDays: 0,
  };
  constructor(
    private deliveryService: deliveryService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.fetchDeliveryServices();
  }

  fetchDeliveryServices() {
    this.deliveryService.getDeliveryServices().subscribe({
      next: (response: any) => {
        if (response.isSuccess) {
          this.deliveryServicesList = response.response as DeliveryService[];
          this.isLoading = false;
        } else {
          console.error('Error fetching delivery services:', response.message);
        }
      },
      error: (error) => {
        console.error('Error fetching delivery services:', error);
      },
    });
  }

  deleteDeliveryService(id: number) {
    if (confirm('Are you sure you want to delete this Service?')) {
      this.deliveryService.deleteDeliveryService(id).subscribe({
        next: () => {
          this.fetchDeliveryServices();
          this.toastr.success('Delivery Service deleted', undefined, {
            timeOut: 5000,
          });
        },
        error: (error) => {
          console.error('Error deleting delivery service:', error);
          this.toastr.error(error, undefined, { timeOut: 5000 });
        },
      });
    }
  }

  editDeliveryService(deliveryService: createDeliveryService) {
    if(deliveryService.deliveryDays<=0)
      {
        this.toastr.error('Please enter valid delivery days', undefined, {
          timeOut: 2000,
        });      
      }

      if(deliveryService.price<=0)
        {
          this.toastr.error('Please enter valid delivery price', undefined, {
            timeOut: 2000,
          });      
        }


    if (
      !deliveryService.imageUrl ||
      !deliveryService.serviceName 
    ) {
      this.toastr.error('Please fill all the fields', undefined, {
        timeOut: 5000,
      });
      return;
    }
    this.deliveryService
      .editDeliveryService(deliveryService, this.editServiceId)
      .subscribe({
        next: (response: any) => {
          if (response.isSuccess) {
            this.toastr.success(
              'Delivery Service successfully updated',
              undefined,
              { timeOut: 5000 }
            );
            this.openEditServicePopup = false;
            this.fetchDeliveryServices();
          } else {
            this.toastr.error('Error editing delivery service', undefined, {
              timeOut: 5000,
            });
          }
        },
        error: (error) => {
          console.error('Error editing delivery service:', error);
        },
      });
  }
  
  createDeliveryService(newDeliveryService: createDeliveryService) {

    if(newDeliveryService.deliveryDays<=0)
      {
        this.toastr.error('Please enter valid delivery days', undefined, {
          timeOut: 2000,
        });      
      }

      if(newDeliveryService.price<=0)
        {
          this.toastr.error('Please enter valid delivery price', undefined, {
            timeOut: 2000,
          });      
        }

    if (
      !newDeliveryService.imageUrl ||
      !newDeliveryService.serviceName 
    ) {
      this.toastr.error('Please fill all the fields', undefined, {
        timeOut: 2000,
      });
      return;
    }


    this.deliveryService.addDeliveryService(newDeliveryService).subscribe({
      next: (response: any) => {
        if (response.isSuccess) {
          this.toastr.success('Delivery service created', undefined, {
            timeOut: 5000,
          });
          this.openAddServicePopup = false;
          this.serviceToAdd.deliveryDays = 0;
          this.serviceToAdd.imageUrl = '';
          this.serviceToAdd.serviceName = '';
          this.serviceToAdd.price = 0;

          this.fetchDeliveryServices();
        } else {
          this.toastr.error('Error creating delivery service', undefined, {
            timeOut: 5000,
          });
        }
      },
      error: (error) => {
        this.toastr.error('Error creating delivery service:', error, {
          timeOut: 5000,
        });
      }
    });
  }

  openAddPopup() {
    this.openAddServicePopup = true;
  }

  openEditPopup(Id: number, service: createDeliveryService) {
    this.editServiceId = Id;
    this.openEditServicePopup = true;
    this.serviceToEdit = { ...service };
  }

  closeAddPopup() {
    this.openAddServicePopup = false;
  }

  closeEditPopup() {
    this.openEditServicePopup = false;
  }
}
