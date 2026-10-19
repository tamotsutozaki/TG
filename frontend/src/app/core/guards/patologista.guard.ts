import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const patologistaGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isLoggedIn() && auth.isPatologista()) return true;

  // Veterinário logado vai para suas solicitações; demais, para o login
  router.navigate([auth.isLoggedIn() ? '/solicitacoes' : '/login']);
  return false;
};
