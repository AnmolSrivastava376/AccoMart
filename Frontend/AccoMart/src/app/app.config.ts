import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { TokenHttpInterceptor } from './services/token-http-interceptor';
import { TokenService } from './services/token.service';
import { routes } from './app.routes';
import { provideToastr } from 'ngx-toastr';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    TokenService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenHttpInterceptor,
      multi: true,
      deps: [TokenService]
    },
    provideToastr(
    {
      closeButton: true,
      positionClass: 'toast-top-right',
      timeOut: 1000000,
      preventDuplicates: false
    }
  )
  ],
};
