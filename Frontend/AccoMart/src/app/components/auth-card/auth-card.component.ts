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
import { TokenHttpInterceptor } from '../../services/token-http-interceptor';
import { TokenService } from '../../services/token.service';


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
    HttpClientModule
  ],
  providers: [HttpService],
  templateUrl: './auth-card.component.html',
  styleUrl: './auth-card.component.css'
})
export class AuthCardComponent {
  builder = inject(FormBuilder);
  httpService = inject(HttpService);
  isLogin = true;
  spinLoader= false;
  constructor(private router: Router, private tokenService : TokenService) {}

  loginForm = this.builder.group({
    email: ['', Validators.required],
    password: ['', Validators.required],
  });
  registerForm = this.builder.group({
    username: ['',Validators.required],
    email: ['', Validators.required],
    password: ['', Validators.required],
  });


  onRegister() {
    const username = this.registerForm.value.username!;
    const email = this.registerForm.value.email!;
    const password = this.registerForm.value.password!;
    console.log({username,email,password})
    this.httpService.register(username,email, password).subscribe((result) => {
      console.log(result)
      this.isLogin=true;
    });
  }
  onLogin() {
    this.spinLoader = true
    const email = this.loginForm.value.email!;
    const password = this.loginForm.value.password!;
    this.httpService.login(email, password).subscribe((result) => {


    this.tokenService.setToken(result.response.accessToken.token);
    this.tokenService.setAccessToken(result.response.accessToken);
    this.tokenService.setRefreshToken(result.response.refreshToken);

    //localStorage.setItem("token", result.response.accessToken.token);

      console.log(result);
      this.router.navigate(['/home']);
    });
  }

  onSwitch() {
    this.isLogin = !this.isLogin
  }

}
