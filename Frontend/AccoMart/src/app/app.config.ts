import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { tokenHttpInterceptor } from './services/token-http-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),
  provideHttpClient(withFetch()),
  provideHttpClient(withInterceptors([tokenHttpInterceptor])),
  ],
};
