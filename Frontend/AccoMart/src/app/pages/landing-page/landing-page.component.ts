import {  HttpClientModule } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { HttpService } from '../../services/http.service';
import { Router } from '@angular/router';
import { productService } from '../../services/product.services';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [
    HttpClientModule
  ],
  providers: [productService],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.css'
})
export class LandingPageComponent {
  constructor(private httpService:productService){}

  router = inject(Router);

  onlogin2FA2()
  {
    this.httpService.onlogin2FA2().subscribe(() => {
      //localStorage.setItem("new1","hello");
      this.router.navigateByUrl('/');
  })
  }

}
