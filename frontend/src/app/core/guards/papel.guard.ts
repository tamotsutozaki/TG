import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { PapelUsuario } from '../models/auth.model';
import { AuthService } from '../services/auth.service';

/**
 * Libera a rota apenas para os papéis informados.
 * Quem está logado com outro papel volta para a própria página inicial.
 */
export function papelGuard(...papeis: PapelUsuario[]): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const papel = auth.currentUser()?.papel;

    if (auth.isLoggedIn() && papel && papeis.includes(papel)) return true;

    return router.parseUrl(auth.isLoggedIn() ? auth.rotaInicial() : '/login');
  };
}
