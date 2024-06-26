import { Component } from '@angular/core';
import { ForgetpasswordComponent } from '../../components/forgetpassword/forgetpassword.component';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-forgetpassword-page',
  standalone: true,
  templateUrl: './forgetpassword-page.component.html',
  styleUrl: './forgetpassword-page.component.css',
  imports: [ForgetpasswordComponent, HttpClientModule],
})
export class ForgetpasswordPageComponent {}
