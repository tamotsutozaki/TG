import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';
import { VinculoService } from '../../../core/services/vinculo.service';
import { LaboratorioBusca } from '../../../core/models/vinculo.model';

// Página aberta pelo link que o laboratório envia aos veterinários
@Component({
  selector: 'app-vincular',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  template: `
    <div class="vincular-container">
      <mat-card class="vincular-card">
        @if (loading) {
          <div class="estado"><mat-spinner diameter="40"></mat-spinner></div>
        } @else if (!laboratorio) {
          <div class="estado">
            <mat-icon class="icone erro">link_off</mat-icon>
            <h2>Link inválido</h2>
            <p>Este laboratório não foi encontrado ou não está disponível.</p>
            <a mat-stroked-button routerLink="/">Ir para o início</a>
          </div>
        } @else {
          <div class="estado">
            <mat-icon class="icone">biotech</mat-icon>
            <h2>{{ laboratorio.nome }}</h2>
            <p class="local">{{ laboratorio.cidade }}/{{ laboratorio.uf }}</p>

            @if (!auth.isLoggedIn()) {
              <p>Entre ou crie sua conta de veterinário para pedir vínculo com este laboratório
                 e enviar solicitações de exame.</p>
              <div class="acoes">
                <a mat-stroked-button color="primary" routerLink="/login" [queryParams]="{ returnUrl: urlAtual }">Entrar</a>
                <a mat-flat-button color="primary" routerLink="/registro" [queryParams]="{ returnUrl: urlAtual }">Criar conta</a>
              </div>
            } @else if (!auth.isVeterinario()) {
              <p>Este link é para médicos veterinários pedirem vínculo com o laboratório.</p>
              <button mat-stroked-button (click)="irParaPainel()">Ir para o painel</button>
            } @else {
              @switch (laboratorio.statusVinculo) {
                @case ('Aprovado') {
                  <p class="status ok"><mat-icon>check_circle</mat-icon> Você já tem vínculo com este laboratório.</p>
                  <a mat-flat-button color="primary" routerLink="/solicitacoes/nova"
                     [queryParams]="{ laboratorio: laboratorio.id }">Nova solicitação</a>
                }
                @case ('Pendente') {
                  <p class="status pendente"><mat-icon>hourglass_top</mat-icon> Pedido enviado. Aguarde a aprovação do laboratório.</p>
                  <a mat-stroked-button routerLink="/laboratorios">Ver meus laboratórios</a>
                }
                @default {
                  @if (laboratorio.statusVinculo === 'Recusado' || laboratorio.statusVinculo === 'Desfeito') {
                    <p class="status">Seu vínculo anterior com este laboratório foi
                      {{ laboratorio.statusVinculo === 'Recusado' ? 'recusado' : 'desfeito' }}. Você pode pedir de novo.</p>
                  } @else {
                    <p>Peça vínculo para enviar solicitações de exame a este laboratório.</p>
                  }
                  @if (erro) { <p class="erro-msg">{{ erro }}</p> }
                  <button mat-flat-button color="primary" (click)="pedirVinculo()" [disabled]="enviando">
                    @if (enviando) { <mat-spinner diameter="20"></mat-spinner> } @else { Pedir vínculo }
                  </button>
                }
              }
            }
          </div>
        }
      </mat-card>
    </div>
  `,
  styles: [`
    .vincular-container {
      display: flex; justify-content: center; align-items: center;
      min-height: 100vh; background: #f5f5f5; padding: 16px;
    }
    .vincular-card { width: 100%; max-width: 460px; padding: 8px; }
    .estado { display: flex; flex-direction: column; align-items: center; text-align: center; gap: 8px; padding: 24px 16px; }
    .estado p { margin: 0; color: #555; line-height: 1.5; }
    .icone { font-size: 52px; width: 52px; height: 52px; color: #3f51b5; }
    .icone.erro { color: #c62828; }
    h2 { margin: 4px 0 0; }
    .local { font-size: 14px; color: #888 !important; margin-bottom: 8px !important; }
    .acoes { display: flex; gap: 12px; margin-top: 12px; }
    .status { display: flex; align-items: center; gap: 6px; }
    .status.ok { color: #2e7d32; }
    .status.pendente { color: #e65100; }
    .estado > a, .estado > button { margin-top: 12px; }
  `]
})
export class VincularComponent implements OnInit {
  auth = inject(AuthService);
  private service = inject(VinculoService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  laboratorio: LaboratorioBusca | null = null;
  loading = true;
  enviando = false;
  erro = '';
  urlAtual = this.router.url;

  ngOnInit() {
    const codigo = this.route.snapshot.paramMap.get('codigo') ?? '';
    this.service.laboratorioPorCodigo(codigo).subscribe({
      next: l => { this.laboratorio = l; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  pedirVinculo() {
    if (!this.laboratorio) return;
    this.enviando = true;
    this.erro = '';
    this.service.solicitar(this.laboratorio.id).subscribe({
      next: v => {
        this.laboratorio!.statusVinculo = v.status;
        this.enviando = false;
      },
      error: err => {
        this.enviando = false;
        this.erro = err?.error?.message ?? 'Não foi possível enviar o pedido.';
      }
    });
  }

  irParaPainel() {
    this.router.navigateByUrl(this.auth.rotaInicial());
  }
}
