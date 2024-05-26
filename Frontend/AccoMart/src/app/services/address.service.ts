import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Address } from "../interfaces/address";

@Injectable({
    providedIn: 'root'
})
export class addressService {
    constructor(private http: HttpClient) { }
    
    getAddressByUserId(userId: string): Observable<any> {
        return this.http.get<any>(`http://localhost:5239/AddressController/GetAddress/${userId}`);
    }
    addAddress(address: Address, userId: string): Observable<Address> {
        return this.http.post<Address>(`http://localhost:5239/AddressController/PostAddress/${userId}`, address);
    }
    getAddressByAddressId(addressId: number): Observable<Address>{
        return this.http.get<Address>(`http://localhost:5239/AddressController/GetAddress/addressId=${addressId}`);
    }

}
