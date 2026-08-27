import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  SolicitacaoDto, SolicitacaoDetalhadaDto,
  UpdateStatusInput, ConsultaPublicaDto
} from '../models/solicitacao.model';

@Injectable({ providedIn: 'root' })
export class SolicitacaoService {
  private url = `${environment.apiUrl}/solicitacoes`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<SolicitacaoDto[]>(this.url);
  }

  getById(id: number) {
    return this.http.get<SolicitacaoDetalhadaDto>(`${this.url}/${id}`);
  }

  create(input: any) {
    return this.http.post<SolicitacaoDto>(this.url, input);
  }

  updateStatus(id: number, input: UpdateStatusInput) {
    return this.http.put<SolicitacaoDetalhadaDto>(`${this.url}/${id}/status`, input);
  }

  consultaPublica(codigo: string) {
    return this.http.get<ConsultaPublicaDto>(`${this.url}/consulta/${codigo}`);
  }
}
