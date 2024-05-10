import { Component, inject } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { HttpService } from '../../services/http.service';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router'; // Import Router
import { Login2FAComponent } from '../login-2-fa/login-2-fa.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    MatInputModule,
    MatCardModule,
    ReactiveFormsModule,
    MatButtonModule,
    HttpClientModule,
    CommonModule,
    Login2FAComponent
  ],
  providers: [HttpService],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  builder = inject(FormBuilder);
  httpService = inject(HttpService);
  // Inject Router
  constructor(private router: Router) {}

  registerForm = this.builder.group({
    username: ['',Validators.required],
    email: ['', Validators.required],
    password: ['', Validators.required],
  });

  onRegister() {
    const username = this.registerForm.value.username!;
    const email = this.registerForm.value.email!;
    const password = this.registerForm.value.password!;
    
    this.httpService.register(username,email, password).subscribe((result) => {
    
      this.router.navigate(['/login']);
    }); 
  }

  onLogin() {
    // Redirect to /register route
    this.router.navigate(['/login']);
  }
}
