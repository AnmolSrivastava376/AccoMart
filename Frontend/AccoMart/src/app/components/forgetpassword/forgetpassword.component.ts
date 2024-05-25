import { Component } from '@angular/core';
import { HttpService } from '../../services/http.service';
import { Route, Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-forgetpassword',
  standalone: true,
  imports: [HttpClientModule,FormsModule,CommonModule],
  providers : [HttpService],
  templateUrl: './forgetpassword.component.html',
  styleUrl: './forgetpassword.component.css'
})
export class ForgetpasswordComponent {
  verificationResponse: any;
  constructor(private httpService: HttpService, private router : Router) { }

  forgotPassword(email: string) {
    // Call your AuthService method to reset the password
    this.httpService.forgotPassword(email)
      .subscribe(
        (response) => {
          // Handle success response
          this.verificationResponse = response.status;
          console.log('Password reset email sent successfully:', response);
           //this.router.navigate(['home/reset-password']);

        },
        (error) => {
          // Handle error response
          console.error('Error sending password reset email:', error);
        }
      );
  }
}
