import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Address } from "../interfaces/address";

@Injectable({
    providedIn: 'root'
})
export class addressService {

    constructor(private http: HttpClient) { }

    getAddress(addressId: number): Observable<Address> {
        return this.http.get<Address>(`http://localhost:5239/AddressController/GetAddress/addressId=${addressId}`);
    }
}
