import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { InsumoDto, CreateInsumoInput } from '../models/insumo.model';

@Injectable({ providedIn: 'root' })
export class InsumoService {
  private url = `${environment.apiUrl}/insumos`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<InsumoDto[]>(this.url);
  }

  getById(id: number) {
    return this.http.get<InsumoDto>(`${this.url}/${id}`);
  }

  create(input: CreateInsumoInput) {
    return this.http.post<InsumoDto>(this.url, input);
  }

  update(id: number, input: any) {
    return this.http.put<InsumoDto>(`${this.url}/${id}`, input);
  }

  delete(id: number) {
    return this.http.delete(`${this.url}/${id}`);
  }

  ajustarQuantidade(id: number, novaQuantidade: number) {
    return this.http.patch<InsumoDto>(`${this.url}/${id}/quantidade`, { novaQuantidade });
  }
}
