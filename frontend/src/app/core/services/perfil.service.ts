import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  AtualizarPerfilLaboratorioInput, AtualizarPerfilVeterinarioInput,
  PerfilLaboratorio, PerfilVeterinario
} from '../models/perfil.model';

@Injectable({ providedIn: 'root' })
export class PerfilService {
  private url = `${environment.apiUrl}/perfil`;

  constructor(private http: HttpClient) {}

  getLaboratorio() {
    return this.http.get<PerfilLaboratorio>(`${this.url}/laboratorio`);
  }

  atualizarLaboratorio(input: AtualizarPerfilLaboratorioInput) {
    return this.http.put<PerfilLaboratorio>(`${this.url}/laboratorio`, input);
  }

  getVeterinario() {
    return this.http.get<PerfilVeterinario>(`${this.url}/veterinario`);
  }

  atualizarVeterinario(input: AtualizarPerfilVeterinarioInput) {
    return this.http.put<PerfilVeterinario>(`${this.url}/veterinario`, input);
  }

  alterarSenha(senhaAtual: string, novaSenha: string) {
    return this.http.put<void>(`${this.url}/senha`, { senhaAtual, novaSenha });
  }
}
