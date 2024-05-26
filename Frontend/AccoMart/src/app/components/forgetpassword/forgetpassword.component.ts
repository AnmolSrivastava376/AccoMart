import { Component } from '@angular/core';
import { HttpService } from '../../services/http.service';
import { Route, Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LoaderComponent } from '../loader/loader.component';

@Component({
  selector: 'app-forgetpassword',
  standalone: true,
  imports: [HttpClientModule,FormsModule,CommonModule,LoaderComponent],
  providers : [HttpService],
  templateUrl: './forgetpassword.component.html',
  styleUrl: './forgetpassword.component.css'
})
export class ForgetpasswordComponent {
  verificationResponse: any;
  spinLoader:boolean= false;
  constructor(private httpService: HttpService, private router : Router) { }

  forgotPassword(email: string) {
    this.spinLoader = true;
    this.httpService.forgotPassword(email)
      .subscribe(
        (response) => {
          this.spinLoader = false;
          this.verificationResponse = response.status;
          console.log('Password reset email sent successfully:', response);

        },
        (error) => {
          // Handle error response
          console.error('Error sending password reset email:', error);
        }
      );
  }
}
