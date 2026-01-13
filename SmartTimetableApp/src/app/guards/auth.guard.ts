import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs/operators';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isSessionValid().pipe(
    map(isValid => {
      if (!isValid) {
        // Redirect to login if session is invalid
        router.navigate(['/login']); 
        return false;
      }
      return true;
    })
  );
};