import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { createDeliveryService } from "../interfaces/createDeliveryService";
import { DeliveryService } from "../interfaces/deliveryService";

@Injectable({
    providedIn: 'root'
})
export class deliveryService {

    constructor(private http: HttpClient) { }

    getDeliveryServices(): Observable<DeliveryService[]> {
        return this.http.get<DeliveryService[]>('http://localhost:5239/DeliveryServiceController/GetAllDeliveryServices');
    }
    getDeliveryDate(deliveryServiceId:number): Observable<number>{
        return this.http.get<number>(`http://localhost:5239/DeliveryServiceController/GetDeliveryDays/${deliveryServiceId}`);
    }
}
