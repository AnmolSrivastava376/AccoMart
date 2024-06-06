import {
  Component,
  Input,
  Output,
  OnInit,
  EventEmitter,
} from '@angular/core';
import { Order } from '../../interfaces/order';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { LoaderComponent } from '../loader/loader.component';
import { addressService } from '../../services/address.service';
import { invoiceService } from '../../services/invoiceService';
import { orderService } from '../../services/order.service';
import { Item } from '../../interfaces/item';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-order-card',
  standalone: true,
  templateUrl: './order-card.component.html',
  styleUrl: './order-card.component.css',
  imports: [CommonModule, HttpClientModule, LoaderComponent],
  providers: [addressService, invoiceService],
})
export class OrderCardComponent implements OnInit {
  @Input() orders?: Order[];
  @Output() Change = new EventEmitter<any>();


  constructor(private invoiceService: invoiceService, private orderService:orderService, private toastr:ToastrService) {}

  downloadFile(data: Blob): void {
    const blob = new Blob([data], { type: 'application/pdf' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'invoice.pdf';
    link.click();
    window.URL.revokeObjectURL(url);
  }

  ngOnInit(): void {}

  format(dateStr: Date): string {
    const date = new Date(dateStr);
    const months = [
      'January',
      'February',
      'March',
      'April',
      'May',
      'June',
      'July',
      'August',
      'September',
      'October',
      'November',
      'December',
    ];
    const month = months[date.getMonth()];
    const day = date.getDate();
    const year = date.getFullYear();

    return `${month} ${day}, ${year}`;
  }

  downloadInvoice(orderId: number): void {
    this.invoiceService.getInvoice(orderId).subscribe({
      next: (response: Blob) => {
        this.downloadFile(response);
      },
      error: (error) => {
        console.error('Error fetching invoice:', error);
      },
    });
  }

  CancelOrder(orderId:number,items:Item[])
  {
     
        this.orderService.cancelOrder(orderId,items).then(response=>{
          this.toastr.success("order cancelled successfully");
          this.Change.emit();

        },error=>{
          this.toastr.error("Cannot cancel order");
        });

  
      
      return;
  }
 
}
