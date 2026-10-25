import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AdminService } from '../../../core/services/admin.service';
import { LaboratorioAdmin } from '../../../core/models/admin.model';
import { confirmar } from '../../../shared/confirmar-dialog.component';
import { formatarCnpj } from '../../../shared/formatadores';

@Component({
  selector: 'app-laboratorios-admin',
  standalone: true,
  imports: [
    CommonModule, RouterLink, MatCardModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatDialogModule, MatSnackBarModule
  ],
  templateUrl: './laboratorios-admin.component.html',
  styleUrl: '../admin-lista.scss'
})
export class LaboratoriosAdminComponent implements OnInit {
  private service = inject(AdminService);
  private dialog = inject(MatDialog);
  private snack = inject(MatSnackBar);

  laboratorios: LaboratorioAdmin[] = [];
  loading = true;
  processandoId: number | null = null;
  formatarCnpj = formatarCnpj;

  ngOnInit() {
    this.service.listarLaboratorios().subscribe({
      next: l => { this.laboratorios = l; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  alternarAtivo(l: LaboratorioAdmin) {
    const desativar = l.ativo;
    confirmar(this.dialog, desativar
      ? {
          titulo: 'Desativar laboratório',
          mensagem: `${l.nome} perde o acesso imediatamente e deixa de aparecer para os veterinários. Os dados são mantidos.`,
          confirmar: 'Desativar',
          perigoso: true
        }
      : {
          titulo: 'Reativar laboratório',
          mensagem: `${l.nome} volta a ter acesso ao sistema.`,
          confirmar: 'Reativar'
        }
    ).subscribe(ok => {
      if (!ok) return;
      this.processandoId = l.id;
      this.service.alterarLaboratorioAtivo(l.id, !desativar).subscribe({
        next: () => {
          l.ativo = !desativar;
          this.processandoId = null;
          this.snack.open(desativar ? 'Laboratório desativado.' : 'Laboratório reativado.', 'OK', { duration: 3000 });
        },
        error: err => {
          this.processandoId = null;
          this.snack.open(err?.error?.message ?? 'Não foi possível alterar.', 'OK', { duration: 4000 });
        }
      });
    });
  }
}
