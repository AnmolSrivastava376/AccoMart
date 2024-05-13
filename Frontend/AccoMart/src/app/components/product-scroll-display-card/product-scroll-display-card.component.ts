import { Component, Input } from '@angular/core';
import { Product } from '../../interfaces/product';
import { CommonModule } from '@angular/common';
import { MatIcon } from '@angular/material/icon';
import { cartItem } from '../../interfaces/cartItem';

@Component({
  selector: 'app-product-scroll-display-card',
  standalone: true,
  imports: [CommonModule,MatIcon],
  templateUrl: './product-scroll-display-card.component.html',
  styleUrl: './product-scroll-display-card.component.css'
})
export class ProductScrollDisplayCardComponent {
  @Input() products?:Product[]
  @Input() cart?:cartItem[]
  i=0
  handleNextButtonClick(){
    if(this.products){
      if(this.i+2<this.products.length){
        this.i++;
      }
    }
  }
  handlePreviousButtonClick(){
    if(this.products){
      if(this.i>0){
        this.i--;
      }
    }
  }
  addToCart(productId: number){
    if(!this.isPresentInCart(productId)){
      this.cart = [...this.cart?this.cart:[], {productId: productId,quantity: 1}]
    }
  }
  isPresentInCart(productId:number):boolean{
    if(this.cart){
      return this.cart.some(item => item.productId === productId);
    }
    else{
      return false;
    }
  }
  findQuantityByProductId(productId: number){
    const item = this.cart?.find(item => item.productId === productId);
    return item ? item.quantity : 0;
  }
  incrementCountByProductId(productId: number){
    this.cart = this.cart?.map(item=>{
      if(item.productId===productId){
        return {
          ...item,
          quantity: item.quantity+1
        };
      }
      else
        return item;
    })
  }
  decrementCountByProductId(productId: number){
    this.cart = this.cart?.map(item=>{
      if(item.productId===productId && item.quantity>1){
        return {
          ...item,
          quantity: item.quantity-1
        };
      }
      else
        return item;
    })
  }
  removeElementByProductId(productId:number){
    this.cart = this.cart?.filter(item=>item.productId!==productId)
  }
}
