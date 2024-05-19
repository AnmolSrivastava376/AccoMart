import { Injectable } from "@angular/core";
import axios from "axios";
import { createDeliveryService } from "../interfaces/createDeliveryService";
import { DeliveryService } from "../interfaces/deliveryService";


@Injectable({
    providedIn:'root'
  })
export class deliveryServices{

     getDeliveryServices(){
        return axios.get<DeliveryService[]>(`http://localhost:5239/DeliveryServiceController/GetAllDeliveryServices`);
     }

     addDeliveryService(deliveryService:createDeliveryService){
      return axios.post('http://localhost:5239/DeliveryServiceController/AddDeliveryService',deliveryService);
     }

     editDeliveryService(deliveryService:createDeliveryService, id:number)     {
      return axios.put(`http://localhost:5239/DeliveryServiceController/UpdateDeliveryService/${id}`,deliveryService);
     }

     deleteDeliveryService(id:number)     {
        return axios.delete(`http://localhost:5239/DeliveryServiceController/DeleteDeliveryService/${id}`);
     }
}
