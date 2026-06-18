import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { trigger, transition, style, animate, query, stagger } from '@angular/animations';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import { DEMO_ACCOUNTS, DemoAccount } from '../../models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  animations: [
    trigger('fadeInUp', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(20px)' }),
        animate('400ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
    trigger('staggerIn', [
      transition(':enter', [
        query('.demo-card', [
          style({ opacity: 0, transform: 'translateY(10px)' }),
          stagger(60, [
            animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
          ]),
        ], { optional: true }),
      ]),
    ]),
  ],
})
export class LoginComponent {
  loginForm: FormGroup;
  hidePassword = true;
  isLoading = false;
  demoAccounts = DEMO_ACCOUNTS;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private toast: ToastService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(3)]],
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const { email, password } = this.loginForm.value;
    this.authService.login({ email, password }).subscribe({
      next: () => {
        this.toast.success('Login successful! Welcome back.');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading = false;
        const message = err.error?.message || 'Login failed. Please check your credentials.';
        this.toast.error(message);
      },
    });
  }

  selectDemoAccount(account: DemoAccount): void {
    if (this.isLoading) {
      return;
    }
    this.loginForm.patchValue({
      email: account.email,
      password: account.password,
    });
    // automatically submit after selecting demo credentials
    this.onSubmit();
  }

  getRoleAvatarClass(role: string): Record<string, boolean> {
    const classMap: Record<string, string> = {
      'Employee': 'bg-blue-500/30 text-blue-200 group-hover:bg-blue-500/50',
      'Manager': 'bg-purple-500/30 text-purple-200 group-hover:bg-purple-500/50',
      'Finance': 'bg-emerald-500/30 text-emerald-200 group-hover:bg-emerald-500/50',
      'IT': 'bg-orange-500/30 text-orange-200 group-hover:bg-orange-500/50',
      'HR': 'bg-pink-500/30 text-pink-200 group-hover:bg-pink-500/50',
      'Admin': 'bg-red-500/30 text-red-200 group-hover:bg-red-500/50',
    };
    return (classMap[role] || classMap['Employee']).split(' ').reduce((acc, cls) => {
      acc[cls] = true;
      return acc;
    }, {} as Record<string, boolean>);
  }

  getRoleBadgeClass(role: string): Record<string, boolean> {
    const classMap: Record<string, string> = {
      'Employee': 'bg-blue-500/20 text-blue-300',
      'Manager': 'bg-purple-500/20 text-purple-300',
      'Finance': 'bg-emerald-500/20 text-emerald-300',
      'IT': 'bg-orange-500/20 text-orange-300',
      'HR': 'bg-pink-500/20 text-pink-300',
      'Admin': 'bg-red-500/20 text-red-300',
    };
    return (classMap[role] || classMap['Employee']).split(' ').reduce((acc, cls) => {
      acc[cls] = true;
      return acc;
    }, {} as Record<string, boolean>);
  }
}
