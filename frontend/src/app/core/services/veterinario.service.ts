import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { VinculoVeterinario } from '../models/veterinario.model';

// Pedidos de vínculo e veterinários vinculados ao laboratório logado
@Injectable({ providedIn: 'root' })
export class VeterinarioService {
  private url = `${environment.apiUrl}/veterinarios`;

  constructor(private http: HttpClient) {}

  listar() {
    return this.http.get<VinculoVeterinario[]>(this.url);
  }

  aprovar(id: number) {
    return this.http.put<VinculoVeterinario>(`${this.url}/${id}/aprovar`, {});
  }

  recusar(id: number) {
    return this.http.put<VinculoVeterinario>(`${this.url}/${id}/recusar`, {});
  }

  desfazer(id: number) {
    return this.http.put<VinculoVeterinario>(`${this.url}/${id}/desfazer`, {});
  }
}
