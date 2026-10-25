import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AdminService } from '../../../core/services/admin.service';
import { VeterinarioAdmin } from '../../../core/models/admin.model';
import { confirmar } from '../../../shared/confirmar-dialog.component';
import { formatarCrmv } from '../../../shared/formatadores';

@Component({
  selector: 'app-veterinarios-admin',
  standalone: true,
  imports: [
    CommonModule, MatCardModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatDialogModule, MatSnackBarModule
  ],
  templateUrl: './veterinarios-admin.component.html',
  styleUrl: '../admin-lista.scss'
})
export class VeterinariosAdminComponent implements OnInit {
  private service = inject(AdminService);
  private dialog = inject(MatDialog);
  private snack = inject(MatSnackBar);

  veterinarios: VeterinarioAdmin[] = [];
  loading = true;
  processandoId: number | null = null;
  formatarCrmv = formatarCrmv;

  ngOnInit() {
    this.service.listarVeterinarios().subscribe({
      next: v => { this.veterinarios = v; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  alternarAtivo(v: VeterinarioAdmin) {
    const desativar = v.ativo;
    confirmar(this.dialog, desativar
      ? {
          titulo: 'Desativar conta',
          mensagem: `${v.nome} perde o acesso imediatamente. O CRMV fica livre para uma nova conta.`,
          confirmar: 'Desativar',
          perigoso: true
        }
      : {
          titulo: 'Reativar conta',
          mensagem: `${v.nome} volta a ter acesso ao sistema.`,
          confirmar: 'Reativar'
        }
    ).subscribe(ok => {
      if (!ok) return;
      this.processandoId = v.id;
      this.service.alterarVeterinarioAtivo(v.id, !desativar).subscribe({
        next: () => {
          v.ativo = !desativar;
          this.processandoId = null;
          this.snack.open(desativar ? 'Conta desativada.' : 'Conta reativada.', 'OK', { duration: 3000 });
        },
        error: err => {
          this.processandoId = null;
          this.snack.open(err?.error?.message ?? 'Não foi possível alterar.', 'OK', { duration: 5000 });
        }
      });
    });
  }
}
