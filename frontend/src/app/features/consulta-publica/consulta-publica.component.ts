import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { environment } from '../../../environments/environment';
import { ConsultaPublicaDto } from '../../core/models/solicitacao.model';

@Component({
  selector: 'app-consulta-publica',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatProgressSpinnerModule, MatIconModule],
  template: `
    <div class="consulta-container">
      <mat-card class="consulta-card">
        <mat-card-header>
          <mat-card-title>LabPat — Consulta de Exame</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          @if (loading) {
            <mat-spinner diameter="40" style="margin: 24px auto"></mat-spinner>
          } @else if (erro) {
            <div class="erro">
              <mat-icon>error_outline</mat-icon>
              <p>{{ erro }}</p>
            </div>
          } @else if (dados) {
            <div class="info-grid">
              <div class="info-item">
                <span class="label">Código</span>
                <span class="value">{{ dados.codigoPublico }}</span>
              </div>
              <div class="info-item">
                <span class="label">Status</span>
                <span class="value status-chip {{ dados.status }}">{{ statusLabel(dados.status) }}</span>
              </div>
              <div class="info-item">
                <span class="label">Tipo de Exame</span>
                <span class="value">{{ dados.tipoExame }}</span>
              </div>
              <div class="info-item">
                <span class="label">Paciente</span>
                <span class="value">{{ dados.pacienteNome }}</span>
              </div>
              <div class="info-item">
                <span class="label">Data de Solicitação</span>
                <span class="value">{{ dados.dataCriacao | date:'dd/MM/yyyy' }}</span>
              </div>
              @if (dados.dataEstimadaConclusao) {
                <div class="info-item">
                  <span class="label">Previsão de Conclusão</span>
                  <span class="value">{{ dados.dataEstimadaConclusao | date:'dd/MM/yyyy' }}</span>
                </div>
              }
            </div>
          }
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .consulta-container {
      display: flex; justify-content: center; align-items: center;
      min-height: 100vh; background: #f5f5f5; padding: 16px;
    }
    .consulta-card { width: 100%; max-width: 480px; padding: 8px; }
    .info-grid { display: grid; gap: 16px; margin-top: 16px; }
    .info-item { display: flex; flex-direction: column; gap: 4px; }
    .label { font-size: 12px; color: #666; }
    .value { font-size: 15px; font-weight: 500; }
    .erro { display: flex; flex-direction: column; align-items: center; color: #c62828; padding: 24px; }
  `]
})
export class ConsultaPublicaComponent {
  dados: ConsultaPublicaDto | null = null;
  loading = true;
  erro = '';

  statusLabels: Record<string, string> = {
    Solicitado: 'Solicitado',
    AguardandoAmostra: 'Aguardando Amostra',
    AmostraRecebida: 'Amostra Recebida',
    EmAnalise: 'Em Análise',
    Concluido: 'Concluído'
  };

  constructor(private route: ActivatedRoute, private http: HttpClient) {
    const codigo = this.route.snapshot.paramMap.get('codigo')!;
    this.http.get<ConsultaPublicaDto>(`${environment.apiUrl}/solicitacoes/consulta/${codigo}`)
      .subscribe({
        next: d => { this.dados = d; this.loading = false; },
        error: () => { this.erro = 'Código não encontrado.'; this.loading = false; }
      });
  }

  statusLabel(s: string) { return this.statusLabels[s] ?? s; }
}
