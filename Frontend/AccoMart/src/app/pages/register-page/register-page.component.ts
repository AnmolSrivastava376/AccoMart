import { Component } from '@angular/core';
import { RegisterComponent } from '../../components/register/register.component';

@Component({
  selector: 'app-register-page',
  standalone: true,
  templateUrl: './register-page.component.html',
  styleUrl: './register-page.component.css',
  imports: [RegisterComponent]
})
export class RegisterPageComponent {

}
