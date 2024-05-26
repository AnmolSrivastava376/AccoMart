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
  imports : [FormsModule,HttpClientModule,LoaderComponent,CommonModule],
  standalone : true,
  providers : [HttpService],
  templateUrl: './reset-password-page.component.html',
  styleUrls: ['./reset-password-page.component.css']
})
export class ResetPasswordPageComponent {
  resetPasswords: resetPassword = { password: '', confirmPassword: '', email: '', token: '' };
  resetResponse: string;
  spinLoader: boolean;

  constructor(private httpService: HttpService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.resetPasswords.token = params['token'] || '';
      this.resetPasswords.email = params['email'] || '';
    });
  }

  resetPassword(password: string, confirmPassword: string) {
    this.spinLoader = true
    this.resetPasswords.password = password;
    this.resetPasswords.confirmPassword = confirmPassword;

    console.log(this.resetPasswords.token, this.resetPasswords.email, this.resetPasswords.password,
      this.resetPasswords.confirmPassword);
    this.httpService.resetPassword(this.resetPasswords)
      .subscribe(
        response => {
          this.resetResponse = 'Password reset successfully.';
          this.router.navigate(['/home/auth']);
        },
        error => {
          console.error('Error resetting password:', error);
        }
      );
  }
}
