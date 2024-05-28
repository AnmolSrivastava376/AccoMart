import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpService } from '../../services/http.service';
import { HttpClientModule } from '@angular/common/http';
import { Router } from '@angular/router';
import { TokenService } from '../../services/token.service';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { Login2FAComponent } from '../../components/login-2-fa/login-2-fa.component';
import { LoaderComponent } from '../../components/loader/loader.component';

@Component({
  selector: 'app-auth-otp',
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
  providers : [HttpService],
  templateUrl: './auth-otp.component.html',
  styleUrl: './auth-otp.component.css'
})
export class AuthOtpComponent {
  builder = inject(FormBuilder);
  httpService = inject(HttpService);
  successMessage: any;
  spinLoader: boolean;
  loginErrorMessage: any;
  constructor(private router: Router, private tokenService : TokenService) {}
  otpForm = this.builder.group({
    email: ['', [Validators.required, Validators.email]],
    otp: ['', [Validators.required, Validators.minLength(6)]],
  });
  sentOtp : boolean = true;

  sendOtp(): void {
    this.spinLoader = true;
    const email: string = String(this.otpForm.value.email);
    this.httpService.loginByEmail(email).subscribe(
      (result: any) => {
        if (result.isSuccess) {
          console.log("OTP Sent");
          this.spinLoader = false;
          this.sentOtp = false;
        } else {
          console.log("OTP is invalid");
        }
      },
      (error) => {
        console.error("Error while sending OTP:", error);
      }
    );
  }

  login()
  {
    this.sentOtp = false;
    const email : string = String(this.otpForm.value.email);
    const otp : string  = String(this.otpForm.value.otp);

  this.httpService.login2FA(otp,email).subscribe(
    (result: any) => {
      if (result.isSuccess) {
        this.tokenService.setToken(result.response.accessToken.token);
        this.tokenService.setAccessToken(result.response.accessToken.token);
        this.tokenService.setRefreshToken(result.response.refreshToken.token);
        this.tokenService.setExpiryAccess(result.response.accessToken.expiryTokenDate);
        this.tokenService.setExpiryRefresh(result.response.refreshToken.expiryTokenDate);
        this.successMessage = result.message;
        this.router.navigate(['/home'])
      } else {
        this.spinLoader = false;
        this.loginErrorMessage = result.message;
        console.error(result.message);
        alert('Unsuccessfull login');
      }
    },
    (error) => {
      this.spinLoader = false;
      this.loginErrorMessage = error.error.message;
      console.error(error);
      alert('Unsuccessfull login');

    }
  );


  }

}
