import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn()) return true;

  // Depois do login, volta para a página que a pessoa tentou abrir
  return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};
