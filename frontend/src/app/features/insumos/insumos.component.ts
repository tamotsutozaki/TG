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
import { InsumoService } from '../../core/services/insumo.service';
import { InsumoDto } from '../../core/models/insumo.model';

// ── Dialog: Criar/Editar Insumo ──────────────────────────────────────────────
@Component({
  selector: 'app-insumo-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ data ? 'Editar' : 'Novo' }} Insumo</h2>
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
          <mat-label>Unidade de Medida</mat-label>
          <input matInput formControlName="unidadeMedida" placeholder="ex: unidade, mL, g">
          @if (form.get('unidadeMedida')?.hasError('required') && form.get('unidadeMedida')?.touched) {
            <mat-error>Obrigatório</mat-error>
          }
        </mat-form-field>
        <mat-form-field appearance="outline">
          <mat-label>Quantidade Mínima (alerta)</mat-label>
          <input matInput formControlName="quantidadeMinima" type="number" min="0">
          @if (form.get('quantidadeMinima')?.hasError('required') && form.get('quantidadeMinima')?.touched) {
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
  styles: [`.form-col { display: flex; flex-direction: column; gap: 4px; min-width: 320px; padding-top: 8px; }`]
})
export class InsumoDialogComponent {
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    public ref: MatDialogRef<InsumoDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: InsumoDto | null
  ) {
    this.form = this.fb.group({
      nome: [data?.nome ?? '', Validators.required],
      unidadeMedida: [data?.unidadeMedida ?? '', Validators.required],
      quantidadeMinima: [data?.quantidadeMinima ?? 0, [Validators.required, Validators.min(0)]]
    });
  }
  salvar() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.ref.close(this.form.value);
  }
}

// ── Dialog: Ajustar Quantidade ───────────────────────────────────────────────
@Component({
  selector: 'app-ajustar-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Ajustar Estoque — {{ data.nome }}</h2>
    <mat-dialog-content>
      <p style="color:#616161; margin-bottom:16px">
        Quantidade atual: <strong>{{ data.quantidadeAtual }} {{ data.unidadeMedida }}</strong>
      </p>
      <form [formGroup]="form">
        <mat-form-field appearance="outline" style="width:100%">
          <mat-label>Nova Quantidade</mat-label>
          <input matInput formControlName="novaQuantidade" type="number" min="0">
          @if (form.get('novaQuantidade')?.hasError('required') && form.get('novaQuantidade')?.touched) {
            <mat-error>Obrigatório</mat-error>
          }
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-stroked-button mat-dialog-close>Cancelar</button>
      <button mat-raised-button color="primary" (click)="salvar()">Salvar</button>
    </mat-dialog-actions>
  `
})
export class AjustarQuantidadeDialogComponent {
  form: FormGroup;
  constructor(
    private fb: FormBuilder,
    public ref: MatDialogRef<AjustarQuantidadeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: InsumoDto
  ) {
    this.form = this.fb.group({
      novaQuantidade: [data.quantidadeAtual, [Validators.required, Validators.min(0)]]
    });
  }
  salvar() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.ref.close(this.form.value.novaQuantidade);
  }
}

// ── Componente principal ─────────────────────────────────────────────────────
@Component({
  selector: 'app-insumos',
  standalone: true,
  imports: [
    CommonModule, MatTableModule, MatButtonModule, MatIconModule,
    MatCardModule, MatSnackBarModule, MatProgressSpinnerModule, MatTooltipModule
  ],
  templateUrl: './insumos.component.html',
  styleUrl: './insumos.component.scss'
})
export class InsumosComponent implements OnInit {
  insumos: InsumoDto[] = [];
  loading = true;
  colunas = ['nome', 'unidade', 'quantidade', 'minima', 'status', 'acoes'];

  constructor(
    private service: InsumoService,
    private dialog: MatDialog,
    private snack: MatSnackBar
  ) {}

  ngOnInit() { this.carregar(); }

  carregar() {
    this.loading = true;
    this.service.getAll().subscribe({
      next: d => { this.insumos = d; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  novo() {
    this.dialog.open(InsumoDialogComponent, { data: null, width: '380px' })
      .afterClosed().subscribe(val => {
        if (!val) return;
        this.service.create(val).subscribe({
          next: () => { this.carregar(); this.snack.open('Insumo criado!', 'OK', { duration: 3000 }); },
          error: () => this.snack.open('Erro ao criar.', 'OK', { duration: 3000 })
        });
      });
  }

  editar(insumo: InsumoDto) {
    this.dialog.open(InsumoDialogComponent, { data: insumo, width: '380px' })
      .afterClosed().subscribe(val => {
        if (!val) return;
        this.service.update(insumo.id, { ...val, ativo: insumo.ativo }).subscribe({
          next: () => { this.carregar(); this.snack.open('Atualizado!', 'OK', { duration: 3000 }); },
          error: () => this.snack.open('Erro.', 'OK', { duration: 3000 })
        });
      });
  }

  ajustar(insumo: InsumoDto) {
    this.dialog.open(AjustarQuantidadeDialogComponent, { data: insumo, width: '360px' })
      .afterClosed().subscribe(novaQtd => {
        if (novaQtd === undefined || novaQtd === null) return;
        this.service.ajustarQuantidade(insumo.id, novaQtd).subscribe({
          next: () => { this.carregar(); this.snack.open('Estoque ajustado!', 'OK', { duration: 3000 }); },
          error: () => this.snack.open('Erro.', 'OK', { duration: 3000 })
        });
      });
  }

  excluir(insumo: InsumoDto) {
    this.service.delete(insumo.id).subscribe({
      next: () => { this.carregar(); this.snack.open('Desativado!', 'OK', { duration: 3000 }); },
      error: () => this.snack.open('Erro.', 'OK', { duration: 3000 })
    });
  }
}
