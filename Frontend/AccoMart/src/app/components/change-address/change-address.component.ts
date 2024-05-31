import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  output,
} from '@angular/core';
import { Router } from '@angular/router';
import { Address } from '../../interfaces/address';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { addressService } from '../../services/address.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-change-address',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './change-address.component.html',
  styleUrls: ['./change-address.component.css'],
  providers: [Router, addressService]

})
export class ChangeAddressComponent implements OnInit {
  newAddress: Address = {
    street: '',
    city: '',
    state: '',
    zipCode: '',
    phoneNumber: '',
    addressId: 0,
  };

  @Input() address: Address[] = [];
  @Input() userId: string;
  showAddressform: boolean = false;
  selectAddress: Address;
  @Output() addressAdded = new EventEmitter<Address>();
  @Output() closeWindow = new EventEmitter<boolean>();
  constructor(private router: Router, private addressService: addressService, private toastr: ToastrService) {}

  ngOnInit(): void {}

  toogleAddressForm() {
    this.showAddressform = !this.showAddressform;
  }

  addAddress() {
    if (this.isAddressValid(this.newAddress)) {
      this.newAddress.zipCode = this.newAddress.zipCode.toString();
      if (
        this.isZipCodeValid(this.newAddress.zipCode) &&
        this.isPhoneNumberValid(this.newAddress.phoneNumber)
      ) {
        this.addressService
          .addAddress(this.newAddress, this.userId)
          .subscribe(() => {
            if (this.address === undefined) this.address = [this.newAddress];
            else this.address.push(this.newAddress);
            this.addressAdded.emit(this.newAddress);
            this.showAddressform = false;
            this.toastr.success('Successfully Added');
            this.refreshAddresss();
          });
      } else {
        this.toastr.error(
          'Please enter a valid 6-digit ZIP code and 10-digit phone number.'
        );
      }
    } else {
      this.toastr.error('Please fill out all address fields.');
    }
  }

  isZipCodeValid(zipCode: string): boolean {
    const zipCodePattern = /^\d{6}$/;
    return zipCodePattern.test(zipCode);
  }

  isPhoneNumberValid(phoneNumber: string): boolean {
    const phoneNumberPattern = /^\d{10}$/;
    return phoneNumberPattern.test(phoneNumber);
  }

  isAddressValid(address: Address): boolean {
    return (
      address.street.trim() !== '' &&
      address.city.trim() !== '' &&
      address.state.trim() !== '' &&
      address.zipCode.toString().trim() !== '' &&
      address.phoneNumber.trim() !== ''
    );
  }

  saveChanges() {
    if (this.selectAddress) {
      this.addressAdded.emit(this.selectAddress);
      this.closeWindow.emit(true);
    } else {
      this.toastr.info('Please select an address.');
    }
  }

  selectedAddress(address: Address) {
    this.selectAddress = { ...address };
  }

  cancel() {
    this.router.navigate(['/home/cart']);
  }

  refreshAddresss() {
    this.addressService
      .getAddressByUserId(this.userId)
      .subscribe((response: any) => (this.address = response.response));
  }
}
