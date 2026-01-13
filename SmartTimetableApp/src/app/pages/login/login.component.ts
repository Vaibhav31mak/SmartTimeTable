import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'] // Check if this matches your file name
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  errorMessage: string = '';
  successMessage: string = ''; // To show "Registered Successfully!"
  
  // Toggle State: true = Login, false = Register
  isLoginMode = true; 

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]], 
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  // Switch between Login and Register
  onSwitchMode() {
    this.isLoginMode = !this.isLoginMode;
    this.errorMessage = '';
    this.successMessage = '';
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    this.errorMessage = '';
    this.successMessage = '';

    const formData = this.loginForm.value;

    if (this.isLoginMode) {
      // --- LOGIN LOGIC ---
      this.authService.login(formData).subscribe({
        next: () => {
          this.router.navigate(['/dashboard']);
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = 'Login failed. Check email or password.';
        }
      });
    } else {
      // --- REGISTER LOGIC ---
      this.authService.register(formData).subscribe({
        next: () => {
          this.successMessage = 'Registration successful! You can now login.';
          this.isLoginMode = true; // Switch back to login view automatically
          this.loginForm.reset();
        },
        error: (err) => {
          console.error(err);
          // Backend returns an array of errors usually
          const msg = err.error?.[0]?.description || 'Registration failed. Email might be taken.';
          this.errorMessage = msg;
        }
      });
    }
  }
}