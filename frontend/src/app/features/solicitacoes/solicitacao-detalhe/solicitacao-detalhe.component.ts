import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatStepperModule } from '@angular/material/stepper';
import { SolicitacaoService } from '../../../core/services/solicitacao.service';
import { LaudoService, LaudoDto } from '../../../core/services/laudo.service';
import { SolicitacaoDetalhadaDto, StatusSolicitacao } from '../../../core/models/solicitacao.model';

@Component({
  selector: 'app-solicitacao-detalhe',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule,
    MatSelectModule, MatFormFieldModule, MatInputModule,
    MatDividerModule, MatSnackBarModule, MatStepperModule
  ],
  templateUrl: './solicitacao-detalhe.component.html',
  styleUrl: './solicitacao-detalhe.component.scss'
})
export class SolicitacaoDetalheComponent implements OnInit {
  solicitacao: SolicitacaoDetalhadaDto | null = null;
  laudo: LaudoDto | null = null;
  loading = true;

  novoStatus = new FormControl<StatusSolicitacao | null>(null, Validators.required);
  observacao = new FormControl('');
  laudoConteudo = new FormControl('', Validators.required);

  atualizandoStatus = false;
  emitindoLaudo = false;
  baixandoPdf = false;
  mostrarFormLaudo = false;

  statusOrdem: StatusSolicitacao[] = [
    'Solicitado', 'AguardandoAmostra', 'AmostraRecebida', 'EmAnalise', 'Concluido'
  ];

  statusLabels: Record<string, string> = {
    Solicitado: 'Solicitado',
    AguardandoAmostra: 'Aguardando Amostra',
    AmostraRecebida: 'Amostra Recebida',
    EmAnalise: 'Em Análise',
    Concluido: 'Concluído'
  };

  proximos: StatusSolicitacao[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private solicitacaoService: SolicitacaoService,
    private laudoService: LaudoService,
    private snack: MatSnackBar
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.carregar(id);
  }

  carregar(id: number) {
    this.loading = true;
    this.solicitacaoService.getById(id).subscribe({
      next: s => {
        this.solicitacao = s;
        this.loading = false;
        this.calcularProximos();
        if (s.status === 'Concluido') this.carregarLaudo(s.id);
      },
      error: () => { this.loading = false; }
    });
  }

  carregarLaudo(sid: number) {
    this.laudoService.getBySolicitacao(sid).subscribe({
      next: l => this.laudo = l,
      error: () => {}
    });
  }

  calcularProximos() {
    if (!this.solicitacao) return;
    const idx = this.statusOrdem.indexOf(this.solicitacao.status);
    this.proximos = this.statusOrdem.slice(idx + 1);
  }

  get statusAtualIndex(): number {
    if (!this.solicitacao) return 0;
    return this.statusOrdem.indexOf(this.solicitacao.status);
  }

  atualizarStatus() {
    if (!this.solicitacao || !this.novoStatus.value) return;
    this.atualizandoStatus = true;
    this.solicitacaoService.updateStatus(this.solicitacao.id, {
      novoStatus: this.novoStatus.value,
      observacao: this.observacao.value ?? undefined
    }).subscribe({
      next: s => {
        this.solicitacao = s;
        this.novoStatus.reset();
        this.observacao.reset();
        this.atualizandoStatus = false;
        this.calcularProximos();
        this.snack.open('Status atualizado!', 'OK', { duration: 3000 });
      },
      error: () => { this.atualizandoStatus = false; }
    });
  }

  emitirLaudo() {
    if (!this.solicitacao || !this.laudoConteudo.value) return;
    this.emitindoLaudo = true;
    this.laudoService.create(this.solicitacao.id, this.laudoConteudo.value).subscribe({
      next: l => {
        this.laudo = l;
        this.emitindoLaudo = false;
        this.mostrarFormLaudo = false;
        this.carregar(this.solicitacao!.id);
        this.snack.open('Laudo emitido!', 'OK', { duration: 3000 });
      },
      error: () => { this.emitindoLaudo = false; }
    });
  }

  downloadPdf() {
    if (!this.laudo) return;
    this.baixandoPdf = true;
    this.laudoService.downloadPdf(this.laudo.id).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `laudo-${this.laudo!.id}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
        this.baixandoPdf = false;
      },
      error: () => { this.baixandoPdf = false; }
    });
  }

  voltar() { this.router.navigate(['/solicitacoes']); }
  statusLabel(s: string) { return this.statusLabels[s] ?? s; }
}
