import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { SolicitacaoService } from '../../core/services/solicitacao.service';
import { InsumoService } from '../../core/services/insumo.service';
import { SolicitacaoDto, StatusSolicitacao } from '../../core/models/solicitacao.model';
import { InsumoDto } from '../../core/models/insumo.model';

interface StatusResumo {
  status: StatusSolicitacao;
  label: string;
  icon: string;
  total: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, RouterLink,
    MatCardModule, MatIconModule, MatButtonModule,
    MatProgressSpinnerModule, MatDividerModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private solicitacaoService = inject(SolicitacaoService);
  private insumoService = inject(InsumoService);
  private router = inject(Router);

  loading = true;

  total = 0;
  emAndamento = 0;
  concluidos = 0;

  porStatus: StatusResumo[] = [];
  prazosVencidos: SolicitacaoDto[] = [];
  estoqueBaixo: InsumoDto[] = [];

  private statusMeta: { status: StatusSolicitacao; label: string; icon: string }[] = [
    { status: 'Solicitado', label: 'Solicitado', icon: 'inbox' },
    { status: 'AguardandoAmostra', label: 'Aguardando Amostra', icon: 'hourglass_empty' },
    { status: 'AmostraRecebida', label: 'Amostra Recebida', icon: 'inventory' },
    { status: 'EmAnalise', label: 'Em Análise', icon: 'biotech' },
    { status: 'Concluido', label: 'Concluído', icon: 'task_alt' }
  ];

  ngOnInit() {
    forkJoin({
      solicitacoes: this.solicitacaoService.getAll(),
      insumos: this.insumoService.getAll()
    }).subscribe({
      next: ({ solicitacoes, insumos }) => {
        this.processar(solicitacoes, insumos);
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  private processar(solicitacoes: SolicitacaoDto[], insumos: InsumoDto[]) {
    this.total = solicitacoes.length;
    this.concluidos = solicitacoes.filter(s => s.status === 'Concluido').length;
    this.emAndamento = this.total - this.concluidos;

    this.porStatus = this.statusMeta.map(m => ({
      ...m,
      total: solicitacoes.filter(s => s.status === m.status).length
    }));

    const hoje = new Date();
    hoje.setHours(0, 0, 0, 0);
    this.prazosVencidos = solicitacoes.filter(s =>
      s.status !== 'Concluido' &&
      s.dataEstimadaConclusao != null &&
      new Date(s.dataEstimadaConclusao) < hoje
    );

    this.estoqueBaixo = insumos.filter(i => i.emEstoqueBaixo);
  }

  abrirSolicitacao(id: number) {
    this.router.navigate(['/solicitacoes', id]);
  }
}
