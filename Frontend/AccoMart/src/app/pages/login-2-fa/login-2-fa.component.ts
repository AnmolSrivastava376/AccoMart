import { Component, inject, numberAttribute } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { HttpService } from '../../services/http.service';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { Int32 } from 'mongodb';

@Component({
  selector: 'app-login-2-fa',
  standalone: true,
  imports: [
    MatInputModule,
    MatCardModule,
    ReactiveFormsModule,
    MatButtonModule,
    HttpClientModule,
    CommonModule
  ],
  providers: [HttpService],
  templateUrl: './login-2-fa.component.html',
  styleUrl: './login-2-fa.component.css'
})
export class Login2FAComponent {
  builder = inject(FormBuilder);
  httpService = inject(HttpService);
 // router = inject(Router);
  loginForm = this.builder.group({
    email: ['', Validators.required],
    otp: [''],
  });

  onlogin2FA()
  {
    const email = this.loginForm.value.email!
    const otp = Number(this.loginForm.value.otp!)
    this.httpService.login2FA(email,otp).subscribe((result) => {
        console.log(result);
    })
  }
}
