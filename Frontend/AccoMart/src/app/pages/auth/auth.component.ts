import { Component } from '@angular/core';
import { LoginComponent } from '../../components/login/login.component';

@Component({
  selector: 'app-auth',
  standalone: true,
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css',
  imports: [LoginComponent]
})
export class AuthComponent {

}
