import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const snack = inject(MatSnackBar);
  const token = auth.getToken();

  if (token) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      // Token expirado ou conta/laboratório desativados: encerra a sessão
      if (err.status === 401 && token) {
        auth.logout();
        snack.open('Sua sessão foi encerrada. Entre novamente.', 'OK', { duration: 5000 });
      }
      return throwError(() => err);
    })
  );
};
