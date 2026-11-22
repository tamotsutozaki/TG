import { HttpErrorResponse } from '@angular/common/http';

// Mensagem que a API devolveu (regra de negócio); se não houver, usa a mensagem padrão
export function mensagemErro(err: unknown, padrao: string): string {
  const corpo = (err as HttpErrorResponse)?.error;
  return (corpo && typeof corpo.message === 'string' && corpo.message) || padrao;
}
