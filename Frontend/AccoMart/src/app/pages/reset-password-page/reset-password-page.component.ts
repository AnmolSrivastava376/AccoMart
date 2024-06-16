import { Component,OnInit } from '@angular/core';
import { resetPassword } from '../../interfaces/resetPassword';
import { HttpService } from '../../services/http.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { LoaderComponent } from '../../components/loader/loader.component';
import * as CryptoJS from 'crypto-js';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-reset-password-page',
  standalone:true,
  imports: [ HttpClientModule, LoaderComponent,CommonModule], 
  providers: [HttpService],
  templateUrl: './reset-password-page.component.html',
  styleUrls: ['./reset-password-page.component.css'],
})
export class ResetPasswordPageComponent implements OnInit {
  inputType = 'password';
  resetPasswordForm: FormGroup;
  resetPasswords: resetPassword = {
    password: '',
    confirmPassword: '',
    email: '',
    token: '',
  };

  resetResponse: string;
  resetError: string;
  spinLoader: boolean;
  key: string = 'test123'; // this will be in .env

  constructor(
    private httpService: HttpService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr:ToastrService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const token = params['token'] || '';
      const email = params['email'] || '';
      const token2 = atob(token.replace(/_/g, '/').replace(/-/g, '+'));
      this.resetPasswords.token = token2;
      this.resetPasswords.email = decodeURIComponent(email);
    });

    
  }

  encryptPassword(password: string, key: string): string {
    //using this to create hash of the password 256 characters long
    const encryptedPassword = CryptoJS.HmacSHA256(password, key).toString()+"PW@"; 
    return encryptedPassword;
  }

  toggleInputType() {
    this.inputType = this.inputType === 'password' ? 'text' : 'password';
  }

  resetPassword(password:string,confirmPassword:string) {
    if (password !== confirmPassword) {
      this.resetError = 'Passwords do not match';
      return;
    }

    if(password.length<6)
    {
        this.resetError = 'Password must be 6 characters long';
        return;
    }

    if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+{}|:"<>?~`\-=[\]\\;',./])[A-Za-z\d!@#$%^&*()_+{}|:"<>?~`\-=[\]\\;',./]{6,}$/.test(password)) {
      this.resetError = 'Password must contain at least one special character';
      return;
    }
    

    this.spinLoader = true;
    const encryptedPassword = this.encryptPassword(password, this.key).toString();
    const encryptedPasswordConfirm = this.encryptPassword(confirmPassword, this.key).toString();
    this.resetPasswords.password = encryptedPassword;
    this.resetPasswords.confirmPassword = encryptedPasswordConfirm;

    this.httpService
      .resetPassword(
        this.resetPasswords.password,
        this.resetPasswords.confirmPassword,
        this.resetPasswords.token,
        this.resetPasswords.email
      )
      .subscribe(
        (response) => {
          this.resetResponse = 'success';
          this.toastr.success("Password reset successful")
          window.location.href = '/home/auth'
        },
        (error) => {
          if (error.error && error.error.errors) {
            this.resetError = error.error.errors.message;
          } else {
            this.resetError = 'Error resetting passwords';
          }

          console.log(error.error);
          this.spinLoader = false;
        }
      );
  }
}
