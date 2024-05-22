import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { Address } from '../../interfaces/address';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { addressService } from '../../services/address.service';

@Component({
  selector: 'app-change-address',
  standalone: true,
  imports:[FormsModule, CommonModule],
  templateUrl: './change-address.component.html',
  styleUrls: ['./change-address.component.css']
})
export class ChangeAddressComponent implements OnInit {

  newAddress: Address = {
    street: '',
    city: '',
    state: '',
    zipCode: '',
    phoneNumber: ''
  };

  // addresses: Address[] = [];
  @Output() addressAdded = new EventEmitter<Address>();

  constructor(private router: Router, private addressService : addressService) { }

  ngOnInit(): void {
    // Load existing address if needed
    // this.address = loadAddressFromService();
  }
 
  addAddress() {
    if (this.isAddressValid(this.newAddress)) {
      // Convert zip code to string
      this.newAddress.zipCode = this.newAddress.zipCode.toString();
  
      this.addressService.addAddress(this.newAddress).subscribe(
        (address) => {
          console.log('Address saved:', address);
          this.addressAdded.emit(address);
          this.router.navigate(['/home/cart']); // Navigate to cart after saving
        },
        (error) => {
          console.error('Error saving address:', error);
          if (error.status === 400 && error.error && error.error.errors) {
            console.error('Validation errors:', error.error.errors);
            alert('Validation errors: ' + JSON.stringify(error.error.errors));
          }
        }
      );
    } else {
      alert('Please fill out all address fields.');
    }
  }
  
  isAddressValid(address: Address): boolean {
    return address.street.trim() !== '' &&
           address.city.trim() !== '' &&
           address.state.trim() !== '' &&
           address.zipCode.toString().trim() !== '' && 
           address.phoneNumber.trim() !== '';
  }

  onSubmit() {
    this.addressService.addAddress(this.newAddress).subscribe(
      (address) => {
        console.log('Address saved:', address);
        this.router.navigate(['/home/cart']); 
      },
      (error) => {
        console.error('Error saving address:', error);
      }
    );
  }


  cancel() {
    this.router.navigate(['/home/cart']); 
  }
}
