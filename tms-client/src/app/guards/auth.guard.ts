import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { RoleName } from '../models';

export const authGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn) {
    router.navigate(['/login']);
    return false;
  }

  const requiredRoles = route.data?.['roles'] as RoleName[] | undefined;
  if (requiredRoles && requiredRoles.length > 0) {
    if (!authService.hasRole(requiredRoles)) {
      router.navigate(['/dashboard']);
      return false;
    }
  }
  
  return true;
};

export const publicGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn) {
    router.navigate(['/dashboard']);
    return false;
  }

  return true;
};
