import { Component } from '@angular/core';
import { resetPassword } from '../../interfaces/resetPassword';
import { HttpService } from '../../services/http.service';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent {
  resetPasswords: resetPassword = { password: '', confirmPassword: '', email: '', token: '' };
  constructor(private httpService: HttpService, private route : ActivatedRoute){ }
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.resetPasswords.token = params['token'] || '';
      this.resetPasswords.email = params['email'] || '';
    });
  }

  resetPassword() {
    this.httpService.resetPassword(this.resetPasswords.token, this.resetPasswords.email, this.resetPasswords)
      .subscribe(
        response => {
          console.log('Password reset successfully.');
          window.location.href = '/home/auth'
        }
      );
  }
}
