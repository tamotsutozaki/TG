import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { MatSelectModule } from '@angular/material/select';
import { forkJoin } from 'rxjs';
import { TipoExameService } from '../../core/services/tipo-exame.service';
import { InsumoService } from '../../core/services/insumo.service';
import { ExameInsumoDto, TipoExameDetalhadoDto, TipoExameDto } from '../../core/models/tipo-exame.model';
import { InsumoDto } from '../../core/models/insumo.model';

// ── Dialog: Criar/Editar Tipo de Exame ──────────────────────────────────────
@Component({
  selector: 'app-tipo-exame-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatProgressSpinnerModule],
  template: `
    <h2 mat-dialog-title>{{ data ? 'Editar' : 'Novo' }} Tipo de Exame</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="form-col">
        <mat-form-field appearance="outline">
          <mat-label>Nome</mat-label>
          <input matInput formControlName="nome">
          @if (form.get('nome')?.hasError('required') && form.get('nome')?.touched) {
            <mat-error>Obrigatório</mat-error>
          }
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Descrição (opcional)</mat-label>
          <textarea matInput formControlName="descricao" rows="2"></textarea>
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Prazo Estimado (dias)</mat-label>
          <input matInput formControlName="prazoEstimadoDias" type="number" min="1">
          @if (form.get('prazoEstimadoDias')?.hasError('required') && form.get('prazoEstimadoDias')?.touched) {
            <mat-error>Obrigatório</mat-error>
          }
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-stroked-button mat-dialog-close>Cancelar</button>
      <button mat-raised-button color="primary" (click)="salvar()">Salvar</button>
    </mat-dialog-actions>
  `,
  styles: [`.form-col { display: flex; flex-direction: column; gap: 4px; width: min(360px, 78vw); padding-top: 8px; }`]
})
export class TipoExameDialogComponent {
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    public ref: MatDialogRef<TipoExameDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoExameDto | null
  ) {
    this.form = this.fb.group({
      nome: [data?.nome ?? '', Validators.required],
      descricao: [data?.descricao ?? ''],
      prazoEstimadoDias: [data?.prazoEstimadoDias ?? 5, [Validators.required, Validators.min(1)]]
    });
  }
  salvar() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.ref.close(this.form.value);
  }
}

// ── Dialog: Gerenciar Templates ──────────────────────────────────────────────
@Component({
  selector: 'app-templates-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatProgressSpinnerModule, MatDividerModule],
  template: `
    <h2 mat-dialog-title>Templates — {{ data.nome }}</h2>
    <mat-dialog-content style="min-width:min(440px, 85vw); max-height:70vh; overflow-y:auto">
      @if (data.templates.length === 0) {
        <p style="color:#9e9e9e; padding: 8px 0">Nenhum template cadastrado.</p>
      }
      @for (t of data.templates; track t.id) {
        <div class="template-box">
          <span class="versao">Versão {{ t.versao }}</span>
          <p>{{ t.conteudo }}</p>
        </div>
      }
      <mat-divider style="margin: 16px 0"></mat-divider>
      <p style="font-weight:500; margin-bottom:8px">Adicionar novo template</p>
      <form [formGroup]="form">
        <mat-form-field appearance="outline" style="width:100%">
          <mat-label>Conteúdo</mat-label>
          <textarea matInput formControlName="conteudo" rows="6"
            placeholder="Estrutura padrão do laudo..."></textarea>
          @if (form.get('conteudo')?.hasError('required') && form.get('conteudo')?.touched) {
            <mat-error>Obrigatório</mat-error>
          }
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-stroked-button mat-dialog-close>Fechar</button>
      <button mat-raised-button color="primary" (click)="addTemplate()" [disabled]="salvando">
        @if (salvando) { Salvando... } @else { Adicionar Template }
      </button>
    </mat-dialog-actions>
  `,
  styles: [`.template-box { background:#f5f5f5; border-radius:8px; padding:12px; margin-bottom:8px; }
    .versao { font-size:11px; color:#757575; }
    p { white-space:pre-wrap; font-size:13px; margin: 4px 0 0; }`]
})
export class TemplatesDialogComponent {
  form: FormGroup;
  salvando = false;
  constructor(
    private fb: FormBuilder,
    public ref: MatDialogRef<TemplatesDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: TipoExameDetalhadoDto,
    private service: TipoExameService
  ) {
    this.form = this.fb.group({ conteudo: ['', Validators.required] });
  }
  addTemplate() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.salvando = true;
    this.service.addTemplate(this.data.id, this.form.value.conteudo).subscribe({
      next: (t: any) => { this.data.templates.unshift(t); this.form.reset(); this.salvando = false; },
      error: () => { this.salvando = false; }
    });
  }
}

// ── Dialog: Insumos consumidos pelo exame ───────────────────────────────────
// Ao emitir o laudo, o estoque desses insumos é descontado automaticamente
@Component({
  selector: 'app-insumos-exame-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatIconModule, MatTooltipModule, MatDividerModule],
  template: `
    <h2 mat-dialog-title>Insumos consumidos — {{ data.tipo.nome }}</h2>
    <mat-dialog-content style="min-width:min(460px, 85vw); max-height:70vh; overflow-y:auto">
      <p class="ajuda">Ao emitir um laudo deste exame, essas quantidades saem do estoque automaticamente.</p>
      @if (vinculados.length === 0) {
        <p class="vazio">Nenhum insumo vinculado.</p>
      }
      @for (v of vinculados; track v.insumoId) {
        <div class="vinculo">
          <span class="nome">{{ v.insumoNome }}</span>
          <span class="qtd">{{ v.quantidadeConsumida }} {{ v.unidadeMedida }} por exame</span>
          <button mat-icon-button color="warn" (click)="remover(v)" [disabled]="salvando" matTooltip="Remover">
            <mat-icon>delete_outline</mat-icon>
          </button>
        </div>
      }
      <mat-divider style="margin: 16px 0"></mat-divider>
      @if (data.insumos.length === 0) {
        <p class="vazio">Cadastre os insumos em Estoque para vinculá-los aqui.</p>
      } @else {
        <p style="font-weight:500; margin-bottom:8px">Vincular insumo</p>
        <form [formGroup]="form" class="linha">
          <mat-form-field appearance="outline" class="campo-insumo">
            <mat-label>Insumo</mat-label>
            <mat-select formControlName="insumoId">
              @for (i of data.insumos; track i.id) {
                <mat-option [value]="i.id">{{ i.nome }} ({{ i.unidadeMedida }})</mat-option>
              }
            </mat-select>
          </mat-form-field>
          <mat-form-field appearance="outline" class="campo-qtd">
            <mat-label>Qtd. por exame</mat-label>
            <input matInput type="number" min="0" step="0.1" formControlName="quantidadeConsumida">
          </mat-form-field>
        </form>
        <p class="ajuda">Se o insumo já estiver vinculado, a quantidade é atualizada.</p>
      }
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-stroked-button mat-dialog-close>Fechar</button>
      @if (data.insumos.length > 0) {
        <button mat-raised-button color="primary" (click)="vincular()" [disabled]="salvando">Vincular</button>
      }
    </mat-dialog-actions>
  `,
  styles: [`.ajuda { font-size:13px; color:#666; margin: 0 0 8px; }
    .vazio { color:#9e9e9e; padding: 4px 0; margin: 0; }
    .vinculo { display:flex; align-items:center; gap:12px; padding:6px 0; border-bottom:1px solid #f0f0f0; }
    .vinculo .nome { flex:1; font-weight:500; }
    .vinculo .qtd { font-size:13px; color:#555; }
    .linha { display:flex; gap:12px; }
    .campo-insumo { flex:2; min-width:0; }
    .campo-qtd { flex:1; min-width:0; }`]
})
export class InsumosExameDialogComponent {
  form: FormGroup;
  salvando = false;
  vinculados: ExameInsumoDto[];
  constructor(
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: { tipo: TipoExameDetalhadoDto; insumos: InsumoDto[] },
    private service: TipoExameService,
    private snack: MatSnackBar
  ) {
    this.vinculados = [...data.tipo.insumos];
    this.form = this.fb.group({
      insumoId: [null, Validators.required],
      quantidadeConsumida: [1, [Validators.required, Validators.min(0.001)]]
    });
  }
  vincular() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    const { insumoId, quantidadeConsumida } = this.form.value;
    this.salvando = true;
    this.service.vincularInsumo(this.data.tipo.id, insumoId, quantidadeConsumida).subscribe({
      next: (v: any) => {
        this.vinculados = [...this.vinculados.filter(x => x.insumoId !== v.insumoId), v];
        this.form.reset({ insumoId: null, quantidadeConsumida: 1 });
        this.salvando = false;
      },
      error: err => {
        this.salvando = false;
        this.snack.open(err?.error?.message ?? 'Não foi possível vincular.', 'OK', { duration: 4000 });
      }
    });
  }
  remover(v: ExameInsumoDto) {
    this.salvando = true;
    this.service.desvincularInsumo(this.data.tipo.id, v.insumoId).subscribe({
      next: () => { this.vinculados = this.vinculados.filter(x => x.insumoId !== v.insumoId); this.salvando = false; },
      error: () => { this.salvando = false; }
    });
  }
}

// ── Componente principal ─────────────────────────────────────────────────────
@Component({
  selector: 'app-tipos-exame',
  standalone: true,
  imports: [
    CommonModule, MatTableModule, MatButtonModule, MatIconModule,
    MatCardModule, MatSnackBarModule, MatProgressSpinnerModule, MatTooltipModule
  ],
  templateUrl: './tipos-exame.component.html',
  styleUrl: './tipos-exame.component.scss'
})
export class TiposExameComponent implements OnInit {
  tipos: TipoExameDto[] = [];
  loading = true;
  colunas = ['nome', 'prazo', 'ativo', 'acoes'];

  constructor(
    private service: TipoExameService,
    private insumoService: InsumoService,
    private dialog: MatDialog,
    private snack: MatSnackBar
  ) {}

  ngOnInit() { this.carregar(); }

  carregar() {
    this.loading = true;
    this.service.getAll().subscribe({
      next: d => { this.tipos = d; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  novo() {
    this.dialog.open(TipoExameDialogComponent, { data: null, width: '420px' })
      .afterClosed().subscribe(val => {
        if (!val) return;
        this.service.create(val).subscribe({
          next: () => { this.carregar(); this.snack.open('Tipo criado!', 'OK', { duration: 3000 }); },
          error: () => this.snack.open('Erro ao criar.', 'OK', { duration: 3000 })
        });
      });
  }

  editar(tipo: TipoExameDto) {
    this.dialog.open(TipoExameDialogComponent, { data: tipo, width: '420px' })
      .afterClosed().subscribe(val => {
        if (!val) return;
        this.service.update(tipo.id, { ...val, ativo: tipo.ativo }).subscribe({
          next: () => { this.carregar(); this.snack.open('Atualizado!', 'OK', { duration: 3000 }); },
          error: () => this.snack.open('Erro.', 'OK', { duration: 3000 })
        });
      });
  }

  excluir(tipo: TipoExameDto) {
    this.service.delete(tipo.id).subscribe({
      next: () => { this.carregar(); this.snack.open('Desativado!', 'OK', { duration: 3000 }); },
      error: () => this.snack.open('Erro.', 'OK', { duration: 3000 })
    });
  }

  insumos(tipo: TipoExameDto) {
    forkJoin({ tipo: this.service.getById(tipo.id), insumos: this.insumoService.getAll() }).subscribe(data =>
      this.dialog.open(InsumosExameDialogComponent, { data, width: '540px', maxHeight: '90vh' })
    );
  }

  templates(tipo: TipoExameDto) {
    this.service.getById(tipo.id).subscribe(det =>
      this.dialog.open(TemplatesDialogComponent, { data: det, width: '500px', maxHeight: '90vh' })
    );
  }
}
