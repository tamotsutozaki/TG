import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { environment } from '../../../environments/environment';
import { ConsultaPublicaDto } from '../../core/models/solicitacao.model';

@Component({
  selector: 'app-consulta-publica',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatProgressSpinnerModule, MatIconModule, MatButtonModule, MatProgressBarModule],
  template: `
    <div class="consulta-container">
      <mat-card class="consulta-card">
        <a mat-button routerLink="/" class="voltar-link">
          <mat-icon>arrow_back</mat-icon>
          Voltar
        </a>
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
            <div class="progresso">
              <div class="progresso-texto">
                <span>Andamento do exame</span>
                <span>Etapa {{ etapa(dados.status) }} de {{ etapas.length }}</span>
              </div>
              <mat-progress-bar mode="determinate" [value]="etapa(dados.status) / etapas.length * 100"></mat-progress-bar>
            </div>
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
                <span class="label">Laboratório</span>
                <span class="value">{{ dados.laboratorioNome }}</span>
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
    .voltar-link { margin: 4px 0 8px; align-self: flex-start; }
    .info-item .status-chip { align-self: flex-start; }
    .info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-top: 16px; }
    @media (max-width: 360px) { .info-grid { grid-template-columns: 1fr; } }
    .info-item { display: flex; flex-direction: column; gap: 4px; }
    .label { font-size: 12px; color: #666; }
    .value { font-size: 15px; font-weight: 500; }
    .progresso { margin-top: 16px; display: flex; flex-direction: column; gap: 8px; }
    .progresso-texto { display: flex; justify-content: space-between; align-items: center; font-size: 13px; color: #666; }
    mat-progress-bar { border-radius: 4px; height: 6px; }
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

  etapas = Object.keys(this.statusLabels);
  etapa(s: string) { return this.etapas.indexOf(s) + 1; }

  statusLabel(s: string) { return this.statusLabels[s] ?? s; }
}
