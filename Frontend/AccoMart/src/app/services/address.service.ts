import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Address } from '../interfaces/address';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class addressService {
  constructor(private http: HttpClient) {}

  baseUrl = environment.serverUrl;

  getAddressByUserId(userId: string): Observable<any> {
    return this.http.get<any>(
      `AddressController/GetAddress/${userId}`
    );
  }

  addAddress(address: Address, userId: string): Observable<Address> {
    return this.http.post<Address>(
      `AddressController/PostAddress/${userId}`,
      address
    );
  }

  getAddressByAddressId(addressId: number): Observable<Address> {
    return this.http.get<Address>(
      `AddressController/GetAddress/addressId=${addressId}`
    );
  }
}
