import { Component } from '@angular/core';
import { ResetPasswordComponent } from '../../components/reset-password/reset-password.component';
import { resetPassword } from '../../interfaces/resetPassword';
import { HttpService } from '../../services/http.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-reset-password-page',
  standalone: true,
  imports: [ResetPasswordComponent],
  templateUrl: './reset-password-page.component.html',
  styleUrl: './reset-password-page.component.css'
})


export class ResetPasswordPageComponent {
  // resetPasswords: resetPassword = { password: '', confirmPassword: '', email: '', token: '' }; // Define the resetPasswords object

  // constructor(private httpService: HttpService, private route : ActivatedRoute,private router : Router ){ }
  // ngOnInit(): void {
  //   // Fetch token and email from route parameters
  //   this.route.queryParams.subscribe(params => {
  //     this.resetPasswords.token = params['token'] || '';
  //     this.resetPasswords.email = params['email'] || '';
  //   });
  // }

  // resetPassword() {
  //   // Call the resetPassword method from the authentication service
  //   this.httpService.resetPassword(this.resetPasswords.token, this.resetPasswords.email, this.resetPasswords)
  //     .subscribe(
  //       response => {
  //         console.log('Password reset successfully.');
  //         this.router.navigate(['/auth/login']);
        

  //       },
  //       error => {
  //         console.error('Error resetting password:', error);
  //       }
  //     );
  // }
}
