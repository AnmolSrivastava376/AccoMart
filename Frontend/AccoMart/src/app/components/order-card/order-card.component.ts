import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Order } from '../../interfaces/order';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { LoaderComponent } from '../loader/loader.component';
import { addressService } from '../../services/address.service';
import { invoiceService } from '../../services/invoiceService';

@Component({
  selector: 'app-order-card',
  standalone: true,
  templateUrl: './order-card.component.html',
  styleUrl: './order-card.component.css',
  imports: [CommonModule, HttpClientModule, LoaderComponent],
  providers: [addressService,invoiceService]
})
export class OrderCardComponent implements OnInit{
  @Input() orders?: Order[];

  constructor(private invoiceService : invoiceService){}
  downloadFile(data: Blob): void {
    const blob = new Blob([data], { type: 'application/pdf' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'invoice.pdf';
    link.click();
    window.URL.revokeObjectURL(url);
  }
  ngOnInit(): void {
  }

  format(dateStr: Date): string {
    const date = new Date(dateStr)
    const months = [
      "January", "February", "March", "April", "May", "June","July", "August", "September", "October", "November", "December"
    ];
    const month = months[date.getMonth()];
    const day = date.getDate();
    const year = date.getFullYear();

    return `${month} ${day}, ${year}`;
  }

  downloadInvoice(orderId : number) : void{
      this.invoiceService.getInvoice(orderId).subscribe(
        (response: Blob) => {
               this.downloadFile(response);
            },
            (error) => {
                console.error('Error fetching invoice:', error);
          }
      )

  }
}
