import { Component } from '@angular/core';
import { HttpService } from '../../services/http.service';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-forgetpassword',
  standalone: true,
  imports: [HttpClientModule],
  providers: [HttpService],
  templateUrl: './forgetpassword.component.html',
  styleUrl: './forgetpassword.component.css',
})
export class ForgetpasswordComponent {
  constructor(private httpService: HttpService) {}

  forgotPassword(email: string) {
    // Call your AuthService method to reset the password
    this.httpService.forgotPassword(email).subscribe((response) => {
      // Handle success response
      console.log('Password reset email sent successfully:', response);
      //  window.location.href = '/home/reset-password';
    });
  }
}
