import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { TipoExameDto, TipoExameDetalhadoDto, CreateTipoExameInput } from '../models/tipo-exame.model';

@Injectable({ providedIn: 'root' })
export class TipoExameService {
  private url = `${environment.apiUrl}/tipos-exame`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<TipoExameDto[]>(this.url);
  }

  getById(id: number) {
    return this.http.get<TipoExameDetalhadoDto>(`${this.url}/${id}`);
  }

  create(input: CreateTipoExameInput) {
    return this.http.post<TipoExameDto>(this.url, input);
  }

  update(id: number, input: any) {
    return this.http.put<TipoExameDto>(`${this.url}/${id}`, input);
  }

  delete(id: number) {
    return this.http.delete(`${this.url}/${id}`);
  }

  addTemplate(id: number, conteudo: string) {
    return this.http.post(`${this.url}/${id}/templates`, { conteudo });
  }

  vincularInsumo(id: number, insumoId: number, quantidadeConsumida: number) {
    return this.http.post(`${this.url}/${id}/insumos`, { insumoId, quantidadeConsumida });
  }

  desvincularInsumo(id: number, insumoId: number) {
    return this.http.delete(`${this.url}/${id}/insumos/${insumoId}`);
  }
}
