import { RouterModule, Routes, RouterLink } from '@angular/router';
import { AuthGuard } from './auth.guard';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { HomeComponent } from './pages/home/home.component';
import { CartComponent } from './pages/cart/cart.component';
import { AuthComponent } from './pages/auth/auth.component';
import { PaymentComponent } from './pages/payment/payment.component';
import { YourOrdersComponent } from './pages/your-orders/your-orders.component';
import { NgModule } from '@angular/core';
import { ChartModule } from 'angular-highcharts';
import { ProductDetailComponent  } from './pages/product-detail/product-detail.component';
import { HttpClientModule } from '@angular/common/http';
import { AdminProductsComponent } from './pages/admin-products/admin-products.component';
import { AdminCategoriesComponent } from './pages/admin-categories/admin-categories.component';
import { InvoicePageComponent } from './pages/invoice-page/invoice-page.component';
import { ForgetpasswordPageComponent } from './pages/forget-password/forgetpassword-page.component';
import { EditProductComponent } from './pages/edit-product/edit-product.component';
import { AddProductComponent } from './pages/add-product/add-product.component';
import { ResetPasswordPageComponent } from './pages/reset-password-page/reset-password-page.component';
import { BuyProductComponent } from './pages/buy-product/buy-product.component';
import { DeliveryServicesComponent } from './pages/delivery-services/delivery-services.component';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';
import { NotfoundComponent } from './components/notfound/notfound.component';
import { AdminDashboardComponent } from './pages/admin-dashboard/admin-dashboard.component';
import { SearchbarComponent } from './components/searchbar/searchbar.component';
import { AuthOtpComponent } from './pages/auth-otp/auth-otp.component';
export const routes: Routes = [
    { path: '', component: LandingPageComponent },
    { path: 'home', component: HomeComponent },
    { path: 'home/productdetail/:productId', component: ProductDetailComponent},
    { path: 'home/cart', component: CartComponent },
    { path: 'home/buy-product/:productId', component: BuyProductComponent},
    { path: 'home/auth', component: AuthComponent },
    { path: 'home/forgotpassword', component:ForgetpasswordPageComponent},
    { path: 'home/reset-password', component: ResetPasswordPageComponent},
    { path: 'home/cart/payment', component: PaymentComponent },
    { path: 'home/yourorders', component: YourOrdersComponent },
    { path: 'home/cart/orders/invoice', component: InvoicePageComponent },
    {path: 'home/auth-otp', component: AuthOtpComponent},
    { path: 'admin', component: AdminDashboardComponent},
    { path: 'admin/products',component:AdminProductsComponent,canActivate: [AuthGuard]},
    { path: 'admin/categories',component:AdminCategoriesComponent,canActivate: [AuthGuard]},
    { path: 'admin/product/edit/:productId',component:EditProductComponent,canActivate: [AuthGuard]},
    { path: 'admin/product/add',component:AddProductComponent,canActivate: [AuthGuard]},
    { path: 'admin/delivery',component:DeliveryServicesComponent,canActivate: [AuthGuard]},
    { path: 'unauthorized',component:UnauthorizedComponent},
    // { path: 'search/:prefix',component: SearchbarComponent},
    { path: '**', component: NotfoundComponent }

];

@NgModule({
    imports: [RouterModule.forRoot(routes), HttpClientModule,ChartModule],
    exports: [RouterModule]
})
export class AppRoutingModule {}
