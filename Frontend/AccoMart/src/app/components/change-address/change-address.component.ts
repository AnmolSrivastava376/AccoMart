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

  @Input() address: Address[];
  @Input() userId: string;
  @Output() addressAdded = new EventEmitter<Address>();

  constructor(private router: Router, private addressService : addressService) { }

  ngOnInit(): void {
    
  }
 
  addAddress() {
    if (this.isAddressValid(this.newAddress)) {
      this.newAddress.zipCode = this.newAddress.zipCode.toString();
      this.addressService.addAddress(this.newAddress, this.userId).subscribe(
        (address) => {
          console.log('Address saved:', address);
          this.addressAdded.emit(address);
          window.location.href = '/home/cart'
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
    this.addressService.addAddress(this.newAddress,this.userId).subscribe(
      (address) => {
        console.log('Address saved:', address);
        this.router.navigate(['/home/cart']); 
      }
    );
  }


  cancel() {
    window.location.href = '/home/cart'
  }
}
