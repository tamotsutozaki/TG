import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { VeterinarioConta } from '../models/veterinario.model';

@Injectable({ providedIn: 'root' })
export class VeterinarioService {
  private url = `${environment.apiUrl}/veterinarios`;

  constructor(private http: HttpClient) {}

  listar() {
    return this.http.get<VeterinarioConta[]>(this.url);
  }

  aprovar(id: number) {
    return this.http.put(`${this.url}/${id}/aprovar`, {});
  }

  recusar(id: number) {
    return this.http.put(`${this.url}/${id}/recusar`, {});
  }
}
