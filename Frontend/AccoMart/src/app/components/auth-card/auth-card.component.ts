import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { Login2FAComponent } from '../login-2-fa/login-2-fa.component';
import { HttpService } from '../../services/http.service';
import { TokenService } from '../../services/token.service';
import { LoaderComponent } from '../loader/loader.component';
import {ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-auth-card',
  standalone: true,
  imports: [
    MatInputModule,
    MatCardModule,
    ReactiveFormsModule,
    MatButtonModule,
    CommonModule,
    Login2FAComponent,
    HttpClientModule,
    LoaderComponent,
    
  ],
  providers: [HttpService],
  templateUrl: './auth-card.component.html',
  styleUrl: './auth-card.component.css',
})
export class AuthCardComponent {
  builder = inject(FormBuilder);
  httpService = inject(HttpService);
  isLogin: boolean = true;
  loginSpinLoader: boolean = false;
  errorMessage: any;
  loginErrorMessage: any;
  successMessage: any;
  registerSpinLoader: boolean;
  inputType = 'password';
  constructor(private router: Router, private tokenService: TokenService,private toastr:ToastrService) {}

  loginForm = this.builder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });
  registerForm = this.builder.group({
    username: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: [
      '',
      [
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(
          /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/
        ),
      ],
    ],
  });

  toogleInputType() {
    if (this.inputType === 'password') {
      this.inputType = 'text';
    } else {
      this.inputType = 'password';
    }
  }

  onRegister() {


    if (this.registerForm.valid) {
      this.registerSpinLoader = true;

      const username: string = String(this.registerForm.value.username);
      const email: string = String(this.registerForm.value.email);
      const password: string = String(this.registerForm.value.password);

      this.httpService.register(username, email, password).subscribe({
        next: (result) => {
          if (result.status === 'Success') {
            this.successMessage = result.message;
            this.registerSpinLoader = false;
            this.toastr.success("Registration successful");
            this.isLogin = true;

          } else {
            console.error('Registration failed:', result.message);
            this.toastr.error("Registration failed");
            this.loginSpinLoader = false;
            this.registerSpinLoader = false;
            this.errorMessage = result.message;
          }
        },
        error: (error) => {
          console.error(error.error.message);
          this.errorMessage = error.error.message;
          this.registerSpinLoader = false;
          this.toastr.error("Registration failed");
        },
        complete: ()=>{
          this.registerSpinLoader = false;
        }
      });
    }
  }

  onLogin() {
    const email: string = String(this.loginForm.value.email);
    const password: string = String(this.loginForm.value.password);
    
    if(email=='' || password=='')
      {
        this.toastr.error("Please enter all fields")
        return;
      }
      this.loginSpinLoader = true;

    this.httpService.login(email, password).subscribe({
      next: (result: any) => {
        if (result.isSuccess) {
          this.tokenService.setToken(result.response.accessToken.token);
          this.tokenService.setAccessToken(result.response.accessToken.token);
          this.tokenService.setRefreshToken(result.response.refreshToken.token);
          this.tokenService.setExpiryAccess(
            result.response.accessToken.expiryTokenDate
          );
          this.tokenService.setExpiryRefresh(
            result.response.refreshToken.expiryTokenDate
          );
          this.successMessage = result.message;
          this.toastr.success("Login success")

          this.router.navigate(['/home']);
        } else {
          this.loginSpinLoader = false;
          this.loginErrorMessage = result.message;
          console.error(result.message);
          this.toastr.error("Unsuccessful Login")
          this.loginSpinLoader = false;

        }
      },
      error: (error) => {
        this.loginSpinLoader = false;
        this.loginErrorMessage = error.message;
        console.error(error);
        this.toastr.error("Unsuccessful Login");
        this.loginSpinLoader = false;

      },
    });
  }
  onSwitch() {
    this.isLogin = !this.isLogin;
  }
}
