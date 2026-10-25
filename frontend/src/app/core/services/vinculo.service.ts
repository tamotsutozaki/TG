import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { LaboratorioBusca, MeuVinculo } from '../models/vinculo.model';

// Laboratórios e vínculos na visão do veterinário
@Injectable({ providedIn: 'root' })
export class VinculoService {
  private api = environment.apiUrl;

  constructor(private http: HttpClient) {}

  buscarLaboratorios(termo = '') {
    const params = termo.trim() ? new HttpParams().set('busca', termo.trim()) : undefined;
    return this.http.get<LaboratorioBusca[]>(`${this.api}/laboratorios`, { params });
  }

  // Público: usado pelo link de vínculo enviado pelo laboratório
  laboratorioPorCodigo(codigo: string) {
    return this.http.get<LaboratorioBusca>(`${this.api}/laboratorios/codigo/${encodeURIComponent(codigo)}`);
  }

  meusVinculos() {
    return this.http.get<MeuVinculo[]>(`${this.api}/vinculos`);
  }

  solicitar(laboratorioId: number) {
    return this.http.post<MeuVinculo>(`${this.api}/vinculos`, { laboratorioId });
  }
}
