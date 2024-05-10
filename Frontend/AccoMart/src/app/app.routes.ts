import { RouterModule, Routes, RouterLink } from '@angular/router';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { HomeComponent } from './pages/home/home.component';
import { CartComponent } from './pages/cart/cart.component';
import { AuthComponent } from './pages/auth/auth.component';
import { PaymentComponent } from './pages/payment/payment.component';
import { YourOrdersComponent } from './pages/your-orders/your-orders.component';
import { InvoiceComponent } from './pages/invoice/invoice.component';
import { Component, NgModule } from '@angular/core';
import {ProductDetailComponent  } from './pages/product-detail/product-detail.component';
import { LoginComponent } from './pages/login/login.component';
import { Login2FAComponent } from './pages/login-2-fa/login-2-fa.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import {RegisterComponent} from './pages/register/register.component'
import { AdminComponent } from './pages/admin/admin.component';
import { AdminProductsComponent } from './pages/admin-products/admin-products.component';
import { AdminCategoriesComponent } from './pages/admin-categories/admin-categories.component';
export const routes: Routes = [
    { path: '', component: LandingPageComponent },
    { path: 'home', component: HomeComponent },
    { path :'home/productdetail/:productId', component: ProductDetailComponent},
    { path: 'home/cart', component: CartComponent },
    { path: 'home/auth', component: AuthComponent },
    { path : 'login', component: LoginComponent},
    { path: 'home/cart/payment', component: PaymentComponent },
    { path: 'home/yourorders', component: YourOrdersComponent },
    { path: 'home/cart/orders/invoice', component: InvoiceComponent },
    { path:  'login-two-factor',component:Login2FAComponent},
    { path:  'register',component:RegisterComponent},
    { path:  'admin',component:AdminComponent},
    { path:  'admin/products',component:AdminProductsComponent},
    { path:  'admin/categories',component:AdminCategoriesComponent}
];

@NgModule({
    imports: [RouterModule.forRoot(routes), HttpClientModule],
    exports: [RouterModule]
})
export class AppRoutingModule {}