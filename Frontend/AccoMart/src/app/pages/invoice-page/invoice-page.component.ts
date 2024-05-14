import { Component } from '@angular/core';
import { InvoiceComponent } from '../../components/invoice/invoice.component';

@Component({
  selector: 'app-invoice-page',
  standalone: true,
  imports: [InvoiceComponent],
  templateUrl: './invoice-page.component.html',
  styleUrl: './invoice-page.component.css'
})
export class InvoicePageComponent {

}
