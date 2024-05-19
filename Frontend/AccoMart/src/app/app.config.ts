import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { TokenHttpInterceptor } from './services/token-http-interceptor';
import { TokenService } from './services/token.service';
import { routes } from './app.routes';
import { cartItemService } from './services/cartItem.services';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    TokenService,
    cartItemService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenHttpInterceptor,
      multi: true,
      deps: [TokenService]
    },
  ],

};
