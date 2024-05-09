import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { HTTP_INTERCEPTORS, provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { TokenHttpInterceptor } from './services/token-http-interceptor';
import { TokenService } from './services/token.service';


export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),
  provideHttpClient(withFetch()),
  TokenService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenHttpInterceptor,
      multi: true,
      deps: [TokenService]
    }
  ],
};

