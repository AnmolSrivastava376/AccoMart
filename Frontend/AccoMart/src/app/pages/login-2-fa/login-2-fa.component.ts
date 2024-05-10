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
import { Router } from '@angular/router';
import { CategoryService } from '../../services/category.services';



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
  router = inject(Router);
  loginForm = this.builder.group({
    email: ['', Validators.required],
    otp: ['',Validators.required],
  });

  onlogin2FA()
  {
    const email = this.loginForm.value.email!
    const otp = this.loginForm.value.otp!
    this.httpService.login2FA(otp,email).subscribe((result) => {
        console.log(result);
        localStorage.setItem("token", result.response.accessToken.token);
        this.router.navigateByUrl('/');
    })

  }

  onlogin2FA2()
  {
    this.httpService.login2FA2().subscribe(() => {
      //localStorage.setItem("new1","hello");
      this.router.navigateByUrl('/');
  })
  }


}
