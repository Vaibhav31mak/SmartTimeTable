import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { tokenInterceptor } from './interceptors/token.interceptor'; // Import your function

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    
    // 1. Setup Router
    provideRouter(routes),

    // 2. Setup HTTP with the Token Interceptor
    provideHttpClient(
      withFetch(), // Use modern Fetch API
      withInterceptors([tokenInterceptor]) // <--- Register the interceptor here
    )
  ]
};