import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Order } from '../../interfaces/order';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-order-card',
  standalone: true,
  imports: [CommonModule,HttpClientModule],
  templateUrl: './order-card.component.html',
  styleUrl: './order-card.component.css'
})
export class OrderCardComponent implements OnChanges{
 ngOnChanges(changes: SimpleChanges): void {
   if(changes['orders']){
    console.log(this.orders)
   }
 }
 @Input() orders?:Order[]
 format(date: Date): string {
  const months = [
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
  ];

  const month = months[date.getMonth()];
  const day = date.getDate();
  const year = date.getFullYear();

  return `${month} ${day}, ${year}`;
}

}
