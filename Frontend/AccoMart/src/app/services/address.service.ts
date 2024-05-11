import { Injectable } from "@angular/core";
import axios from "axios";
import { Address } from "../interfaces/address";



@Injectable({
    providedIn:'root'
  })
export class addressService{

     getAddress(addressId : number){
        return axios.get<Address>(`http://localhost:5239/AddressController/GetAddress/addressId=${addressId}`);
     }
}
