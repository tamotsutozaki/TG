import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface ExtrairSolicitacaoDto {
  vetNome: string | null;
  vetCrmvNumero: string | null;
  vetCrmvEstado: string | null;
  vetEmail: string | null;
  vetTelefone: string | null;
  tutorNome: string | null;
  tutorTelefone: string | null;
  tutorEmail: string | null;
  pacienteNome: string | null;
  especie: string | null;
  raca: string | null;
  sexo: string | null;
  idade: string | null;
  peso: string | null;
  tipoExame: string | null;
  descricaoClinica: string | null;
}

@Injectable({ providedIn: 'root' })
export class GeminiService {
  constructor(private http: HttpClient) {}

  extrairDados(arquivo: File) {
    const form = new FormData();
    form.append('arquivo', arquivo);
    return this.http.post<ExtrairSolicitacaoDto>(
      `${environment.apiUrl}/gemini/extrair`, form
    );
  }
}
