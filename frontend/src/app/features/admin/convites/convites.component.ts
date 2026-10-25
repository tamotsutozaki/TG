import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Clipboard } from '@angular/cdk/clipboard';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AdminService } from '../../../core/services/admin.service';
import { Convite, ConviteCriado } from '../../../core/models/admin.model';
import { confirmar } from '../../../shared/confirmar-dialog.component';
import { linkAbsoluto } from '../../../shared/formatadores';

@Component({
  selector: 'app-convites',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatTooltipModule,
    MatDialogModule, MatSnackBarModule
  ],
  templateUrl: './convites.component.html',
  styleUrl: './convites.component.scss'
})
export class ConvitesComponent implements OnInit {
  private service = inject(AdminService);
  private clipboard = inject(Clipboard);
  private dialog = inject(MatDialog);
  private snack = inject(MatSnackBar);

  readonly validades = [1, 3, 7, 15, 30];

  form = inject(FormBuilder).group({
    descricao: [''],
    validadeDias: [7, Validators.required]
  });

  convites: Convite[] = [];
  loading = true;
  gerando = false;
  revogandoId: number | null = null;

  // Convite recém-gerado: o link só pode ser exibido agora
  criado: ConviteCriado | null = null;

  ngOnInit() {
    this.carregar();
  }

  carregar() {
    this.service.listarConvites().subscribe({
      next: c => { this.convites = c; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  get linkCriado(): string {
    return this.criado ? linkAbsoluto(`/cadastro-laboratorio?convite=${this.criado.token}`) : '';
  }

  gerar() {
    if (this.form.invalid) return;
    this.gerando = true;
    const { descricao, validadeDias } = this.form.getRawValue();
    this.service.gerarConvite(descricao?.trim() || null, validadeDias!).subscribe({
      next: c => {
        this.criado = c;
        this.gerando = false;
        this.form.reset({ descricao: '', validadeDias: 7 });
        this.carregar();
      },
      error: err => {
        this.gerando = false;
        this.snack.open(err?.error?.message ?? 'Não foi possível gerar o convite.', 'OK', { duration: 4000 });
      }
    });
  }

  copiar() {
    if (this.clipboard.copy(this.linkCriado)) {
      this.snack.open('Link copiado! Envie ao laboratório.', 'OK', { duration: 3000 });
    }
  }

  revogar(c: Convite) {
    confirmar(this.dialog, {
      titulo: 'Revogar convite',
      mensagem: `O link do convite${c.descricao ? ` "${c.descricao}"` : ''} deixará de funcionar.`,
      confirmar: 'Revogar',
      perigoso: true
    }).subscribe(ok => {
      if (!ok) return;
      this.revogandoId = c.id;
      this.service.revogarConvite(c.id).subscribe({
        next: () => {
          this.revogandoId = null;
          if (this.criado?.id === c.id) this.criado = null;
          this.carregar();
          this.snack.open('Convite revogado.', 'OK', { duration: 3000 });
        },
        error: err => {
          this.revogandoId = null;
          this.snack.open(err?.error?.message ?? 'Não foi possível revogar.', 'OK', { duration: 4000 });
        }
      });
    });
  }
}
