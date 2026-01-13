import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router'; // Import Router
import { AuthService } from '../services/auth.service';
import { catchError, throwError } from 'rxjs'; // Import RxJS operators

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router); // Inject Router
  const token = authService.getToken();

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // If the backend says "401 Unauthorized", force logout
      if (error.status === 401) {
        authService.logout(); // Clear token
        router.navigate(['/login']); // Go to login page
      }
      return throwError(() => error);
    })
  );
};