import { Component, Input } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar.component';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-invoice',
  standalone: true,
  imports: [NavbarComponent,HttpClientModule],
  providers:[],
  templateUrl: './invoice.component.html',
  styleUrl: './invoice.component.css'
})
export class InvoiceComponent {
    
}
