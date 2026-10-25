import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Convite, ConviteCriado, LaboratorioAdmin, VeterinarioAdmin } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private url = `${environment.apiUrl}/admin`;

  constructor(private http: HttpClient) {}

  listarConvites() {
    return this.http.get<Convite[]>(`${this.url}/convites`);
  }

  gerarConvite(descricao: string | null, validadeDias: number) {
    return this.http.post<ConviteCriado>(`${this.url}/convites`, { descricao, validadeDias });
  }

  revogarConvite(id: number) {
    return this.http.put(`${this.url}/convites/${id}/revogar`, {});
  }

  listarLaboratorios() {
    return this.http.get<LaboratorioAdmin[]>(`${this.url}/laboratorios`);
  }

  alterarLaboratorioAtivo(id: number, ativo: boolean) {
    return this.http.put(`${this.url}/laboratorios/${id}/ativo`, { ativo });
  }

  listarVeterinarios() {
    return this.http.get<VeterinarioAdmin[]>(`${this.url}/veterinarios`);
  }

  alterarVeterinarioAtivo(id: number, ativo: boolean) {
    return this.http.put(`${this.url}/veterinarios/${id}/ativo`, { ativo });
  }
}
