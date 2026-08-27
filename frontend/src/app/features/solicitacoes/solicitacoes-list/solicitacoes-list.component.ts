import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { SolicitacaoService } from '../../../core/services/solicitacao.service';
import { SolicitacaoDto } from '../../../core/models/solicitacao.model';

@Component({
  selector: 'app-solicitacoes-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule, MatButtonModule, MatIconModule,
    MatCardModule, MatProgressSpinnerModule, MatTooltipModule
  ],
  templateUrl: './solicitacoes-list.component.html',
  styleUrl: './solicitacoes-list.component.scss'
})
export class SolicitacoesListComponent implements OnInit {
  solicitacoes: SolicitacaoDto[] = [];
  loading = true;
  colunas = ['codigoPublico', 'tipoExame', 'paciente', 'vet', 'status', 'data', 'acoes'];

  statusLabels: Record<string, string> = {
    Solicitado: 'Solicitado',
    AguardandoAmostra: 'Aguardando Amostra',
    AmostraRecebida: 'Amostra Recebida',
    EmAnalise: 'Em Análise',
    Concluido: 'Concluído'
  };

  constructor(private service: SolicitacaoService, private router: Router) {}

  ngOnInit() {
    this.service.getAll().subscribe({
      next: data => { this.solicitacoes = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  nova() { this.router.navigate(['/solicitacoes/nova']); }
  abrir(id: number) { this.router.navigate(['/solicitacoes', id]); }
  statusLabel(s: string) { return this.statusLabels[s] ?? s; }
}
