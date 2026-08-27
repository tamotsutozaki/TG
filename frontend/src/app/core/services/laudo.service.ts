import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface LaudoDto {
  id: number;
  solicitacaoId: number;
  codigoSolicitacao: string;
  tipoExame: string;
  pacienteNome: string;
  conteudo: string;
  patologistaNome: string;
  emitidoEm: string;
}

@Injectable({ providedIn: 'root' })
export class LaudoService {
  private url = `${environment.apiUrl}/laudos`;

  constructor(private http: HttpClient) {}

  create(solicitacaoId: number, conteudo: string) {
    return this.http.post<LaudoDto>(this.url, { solicitacaoId, conteudo });
  }

  getById(id: number) {
    return this.http.get<LaudoDto>(`${this.url}/${id}`);
  }

  getBySolicitacao(solicitacaoId: number) {
    return this.http.get<LaudoDto>(`${this.url}/solicitacao/${solicitacaoId}`);
  }

  downloadPdf(id: number) {
    return this.http.get(`${this.url}/${id}/pdf`, { responseType: 'blob' });
  }
}
