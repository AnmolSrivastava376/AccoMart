import { Injectable } from '@angular/core';
import { cartItem } from '../interfaces/cartItem';

@Injectable({
  providedIn: 'root',
})
export class BuyNowService {
  private _product: cartItem;
  constructor() {}

  get item(): cartItem {
    return this._product;
  }

  set item(product: cartItem) {
    this._product = product;
    this.storeProductInSessionStorage(product);
  }

  private storeProductInSessionStorage(product: cartItem): void {
    const productString = JSON.stringify(product);
    sessionStorage.setItem('productItem', productString);
  }

  getProductFromSessionStorage(): cartItem | null {
    const productString = sessionStorage.getItem('productItem');
    if (productString) {
      const product: cartItem = JSON.parse(productString);
      return product;
    } else {
      return null;
    }
  }
  removeItem() {
    sessionStorage.removeItem('productItem');
  }
}