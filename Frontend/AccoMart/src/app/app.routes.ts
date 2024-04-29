import { RouterModule, Routes } from '@angular/router';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { HomeComponent } from './pages/home/home.component';
import { CartComponent } from './pages/cart/cart.component';
import { AuthComponent } from './pages/auth/auth.component';
import { OrdersComponent } from './pages/orders/orders.component';
import { PaymentComponent } from './pages/payment/payment.component';
import { YourOrdersComponent } from './pages/your-orders/your-orders.component';
import { InvoiceComponent } from './pages/invoice/invoice.component';
import { NgModule } from '@angular/core';
import {ProductDetailComponent  } from './pages/product-detail/product-detail.component';


export const routes: Routes = [
    { path: '', component: LandingPageComponent },
    { path: 'home', component: HomeComponent },
    {path :'home/productdetail', component: ProductDetailComponent },
    { path: 'home/cart', component: CartComponent },
    { path: 'home/auth', component: AuthComponent },
    { path: 'home/cart/orders', component: OrdersComponent },
    { path: 'home/cart/payment', component: PaymentComponent },
    { path: 'home/yourorders', component: YourOrdersComponent },
    { path: 'home/cart/orders/invoice', component: InvoiceComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {} 