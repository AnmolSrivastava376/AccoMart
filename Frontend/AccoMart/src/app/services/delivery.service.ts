import { Injectable } from "@angular/core";
import axios from "axios";
import { DeliveryService } from "../interfaces/deliveryService";



@Injectable({
    providedIn:'root'
  })
export class deliveryServices{

     getDeliveryServices(){
        return axios.get<DeliveryService[]>(`http://localhost:5239/DeliveryServiceController/GetAllDeliveryServices`);
     }


}
