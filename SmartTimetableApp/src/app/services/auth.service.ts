import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {
  // Use 'inject' for cleaner dependency injection in v19
  private http = inject(HttpClient);
  private router = inject(Router);

  // 🔴 CHANGE THIS PORT TO MATCH YOUR RUNNING .NET API
  private apiUrl = 'http://localhost:5192/api/auth'; 
  private tokenKey = 'authToken';

  login(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, data).pipe(
      tap(res => localStorage.setItem(this.tokenKey, res.token))
    );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login']);
  }
// --- ADD THIS NEW METHOD ---
register(credentials: any) {
  return this.http.post<any>(`${this.apiUrl}/register`, credentials);
}
  getToken() { return localStorage.getItem(this.tokenKey); }

  isSessionValid(): Observable<boolean> {
    const token = this.getToken();
    if (!token) return of(false); 

    // We expect the .NET backend to return 200 OK if token is valid
    return this.http.get(`${this.apiUrl}/validate-session`).pipe(
      map(() => true),
      catchError(() => {
        this.logout();
        return of(false);
      })
    );
  }
}