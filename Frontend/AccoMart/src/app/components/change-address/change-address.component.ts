import { HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-change-address',
  standalone: true,
  imports: [HttpClientModule],
  templateUrl: './change-address.component.html',
  styleUrl: './change-address.component.css'
})
export class ChangeAddressComponent {

}
