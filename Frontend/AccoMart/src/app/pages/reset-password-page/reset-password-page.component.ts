import { Component } from '@angular/core';
import { resetPassword } from '../../interfaces/resetPassword';
import { HttpService } from '../../services/http.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { LoaderComponent } from '../../components/loader/loader.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-reset-password-page',
  imports: [FormsModule, HttpClientModule, LoaderComponent, CommonModule],
  standalone: true,
  providers: [HttpService],
  templateUrl: './reset-password-page.component.html',
  styleUrls: ['./reset-password-page.component.css'],
})
export class ResetPasswordPageComponent {
  inputType = 'password';
  resetPasswords: resetPassword = {
    password: '',
    confirmPassword: '',
    email: '',
    token: '',
  };
  resetResponse: string;
  resetError: string;
  spinLoader: boolean;
  constructor(
    private httpService: HttpService,
    private route: ActivatedRoute,
    private router: Router
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
  toogleInputType() {
    if (this.inputType === 'password') {
      this.inputType = 'text';
    } else {
      this.inputType = 'password';
    }
  }
  resetPassword(password: string, confirmPassword: string) {
    if (this.resetPasswords.confirmPassword !== this.resetPasswords.password) {
      this.resetError = 'Passwords do not match';
    } else {
      this.spinLoader = true;
      this.resetPasswords.password = password;
      this.resetPasswords.confirmPassword = confirmPassword;
      console.log(
        this.resetPasswords.password,
        this.resetPasswords.confirmPassword,
        this.resetPasswords.token,
        this.resetPasswords.email
      );
      this.httpService
        .resetPassword(
          this.resetPasswords.password,
          this.resetPasswords.confirmPassword,
          this.resetPasswords.token,
          this.resetPasswords.email
        )
        .subscribe(
          () => {
            this.resetResponse = 'Password reset successfully.';
            this.router.navigate(['/home/auth']);
          },
          () => {
            this.resetError = 'Error resetting passwords';
            this.spinLoader = false;
          }
        );
    }
  }
}
