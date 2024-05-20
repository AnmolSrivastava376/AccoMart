import { Injectable } from '@angular/core';
import { CartHttpService } from './cart.http.service';

@Injectable({
  providedIn: 'root',
})
export class cartItemService {
  constructor(private cartHttpService: CartHttpService){}
}
