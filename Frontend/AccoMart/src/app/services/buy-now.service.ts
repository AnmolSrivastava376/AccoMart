import { Injectable } from "@angular/core";
import { cartItem } from "../interfaces/cartItem";

@Injectable({
    providedIn: 'root'
})
export class BuyNowService {
    private _product: cartItem;

    constructor() {}

    get item(): cartItem {
        return this._product;
    }

    set item(product: cartItem) {
        this._product = product;
        this.storeProductInLocalStorage(product);
    }

    private storeProductInLocalStorage(product: cartItem): void {
        const productString = JSON.stringify(product);
        localStorage.setItem('productItem', productString);
    }

    getProductFromLocalStorage(): cartItem | null {
        const productString = localStorage.getItem('productItem');
        if (productString) {
            const product: cartItem = JSON.parse(productString);
            return product;
        } else {
            return null;
        }
    }
    removeItem(){
        localStorage.removeItem('productItem')
    }
}
