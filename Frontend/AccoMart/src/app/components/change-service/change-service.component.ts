import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { DeliveryService } from '../../interfaces/deliveryService';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-change-service',
  standalone: true,
  imports: [CommonModule,HttpClientModule],
  templateUrl: './change-service.component.html',
  styleUrl: './change-service.component.css'
})
export class ChangeServiceComponent implements OnInit{
  @Input() delivery: DeliveryService[]
  @Input() index: number
  @Output() activeDeliveryIndex = new EventEmitter<number>();
  @Output() closeWindow = new EventEmitter<boolean>();
  activeIndex:number;
  ngOnInit(): void {
      this.activeIndex = this.index
  }
  onChangeDelivery(index: number) {
    if (index >= 0 && index < this.delivery.length) {
      this.activeIndex = index
    }
  }
  onSaveClick(){
    this.activeDeliveryIndex.emit(this.activeIndex);
    this.closeWindow.emit(true);
  }
}
