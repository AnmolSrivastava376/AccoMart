import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { Router } from '@angular/router';
import { Login2FAComponent } from '../../pages/login-2-fa/login-2-fa.component';
import { HttpService } from '../../services/http.service';
import { TokenService } from '../../services/token.service';
import { LoaderComponent } from '../loader/loader.component';


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
    LoaderComponent
  ],
  providers: [HttpService],
  templateUrl: './auth-card.component.html',
  styleUrl: './auth-card.component.css'
})
export class AuthCardComponent {
  builder = inject(FormBuilder);
  httpService = inject(HttpService);
  isLogin:boolean= true;
  spinLoader:boolean= false;
  errorMessage: any;
  constructor(private router: Router, private tokenService : TokenService) {}

  loginForm = this.builder.group({
    email: ['', Validators.required],
    password: ['', Validators.required],
  });
  registerForm = this.builder.group({
    username: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/)]],
  });


  onRegister() {
    if (this.registerForm.valid) {
      const username: string = String(this.registerForm.value.username);
      const email: string = String(this.registerForm.value.email);
      const password: string = String(this.registerForm.value.password);

        this.httpService.register(username, email, password).subscribe(
            (result) => {
                console.log(result);
                if (result.status === 'Success') {
                    this.isLogin = true;
                } else {
                    console.error('Registration failed:', result.message);
                    this.errorMessage = result.message;
                }
            },
            (error) => {
                console.error(error.error.message);
                this.errorMessage = error.error.message;
            }
        );
    }
}



  onLogin() {
    this.spinLoader = true
    const email = this.loginForm.value.email!;
    const password = this.loginForm.value.password!;
    this.httpService.login(email, password).subscribe((result) => {
    this.tokenService.setToken(result.response.accessToken.token);
    this.tokenService.setAccessToken(result.response.accessToken.token);
    this.tokenService.setRefreshToken(result.response.refreshToken.token);
    this.tokenService.setExpiryAccess(result.response.accessToken.expiryTokenDate);
    this.tokenService.setExpiryRefresh(result.response.refreshToken.expiryTokenDate);
    console.log(result);
    window.location.href = '/home'
    });
  }
  onSwitch() {
    this.isLogin = !this.isLogin
  }
}
