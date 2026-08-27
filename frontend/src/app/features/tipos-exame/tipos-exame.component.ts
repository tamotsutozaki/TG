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
import { TipoExameService } from '../../core/services/tipo-exame.service';
import { TipoExameDetalhadoDto, TipoExameDto } from '../../core/models/tipo-exame.model';

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
  styles: [`.form-col { display: flex; flex-direction: column; gap: 4px; min-width: 360px; padding-top: 8px; }`]
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
    <mat-dialog-content style="min-width:440px; max-height:70vh; overflow-y:auto">
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

  templates(tipo: TipoExameDto) {
    this.service.getById(tipo.id).subscribe(det =>
      this.dialog.open(TemplatesDialogComponent, { data: det, width: '500px', maxHeight: '90vh' })
    );
  }
}
