import { Component, inject } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { HttpService } from '../../services/http.service';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { Login2FAComponent } from '../login-2-fa/login-2-fa.component';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    MatInputModule,
    MatCardModule,
    ReactiveFormsModule,
    MatButtonModule,
    HttpClientModule,
    CommonModule,
    Login2FAComponent
  ],
  providers: [HttpService],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  builder = inject(FormBuilder);
  httpService = inject(HttpService);
 // router = inject(Router);
  loginForm = this.builder.group({
    email: ['', Validators.required],
    password: ['', Validators.required],
  });
  onLogin()
  {
    const email = this.loginForm.value.email!
    const password = this.loginForm.value.password!
    this.httpService.login(email,password).subscribe((result) => {

    })
  }
}
