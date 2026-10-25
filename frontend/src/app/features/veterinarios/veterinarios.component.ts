import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Clipboard } from '@angular/cdk/clipboard';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { VeterinarioService } from '../../core/services/veterinario.service';
import { PerfilService } from '../../core/services/perfil.service';
import { VinculoVeterinario } from '../../core/models/veterinario.model';
import { STATUS_VINCULO_LABEL, StatusVinculo } from '../../core/models/vinculo.model';
import { confirmar } from '../../shared/confirmar-dialog.component';
import { formatarCrmv, linkAbsoluto } from '../../shared/formatadores';

// Pedidos de vínculo e veterinários vinculados ao laboratório
@Component({
  selector: 'app-veterinarios',
  standalone: true,
  imports: [
    CommonModule, MatCardModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatDialogModule, MatSnackBarModule
  ],
  templateUrl: './veterinarios.component.html',
  styleUrl: './veterinarios.component.scss'
})
export class VeterinariosComponent implements OnInit {
  private service = inject(VeterinarioService);
  private perfilService = inject(PerfilService);
  private clipboard = inject(Clipboard);
  private dialog = inject(MatDialog);
  private snack = inject(MatSnackBar);

  vinculos: VinculoVeterinario[] = [];
  loading = true;
  processandoId: number | null = null;
  linkVinculo = '';
  formatarCrmv = formatarCrmv;

  ngOnInit() {
    this.carregar();
    this.perfilService.getLaboratorio().subscribe({
      next: p => this.linkVinculo = linkAbsoluto(`/vincular/${p.codigoVinculo}`)
    });
  }

  carregar() {
    this.service.listar().subscribe({
      next: v => { this.vinculos = v; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  get pendentes() { return this.vinculos.filter(v => v.status === 'Pendente'); }
  get vinculados() { return this.vinculos.filter(v => v.status === 'Aprovado'); }
  get historico() { return this.vinculos.filter(v => v.status === 'Recusado' || v.status === 'Desfeito'); }

  label(status: StatusVinculo) { return STATUS_VINCULO_LABEL[status]; }

  copiarLink() {
    if (this.clipboard.copy(this.linkVinculo)) {
      this.snack.open('Link copiado! Envie aos veterinários.', 'OK', { duration: 3000 });
    }
  }

  aprovar(v: VinculoVeterinario) {
    this.executar(v, this.service.aprovar(v.id), `${v.nome} vinculado ao laboratório.`);
  }

  recusar(v: VinculoVeterinario) {
    this.executar(v, this.service.recusar(v.id), `Pedido de ${v.nome} recusado.`);
  }

  desfazer(v: VinculoVeterinario) {
    confirmar(this.dialog, {
      titulo: 'Desfazer vínculo',
      mensagem: `${v.nome} não poderá mais enviar solicitações ao laboratório. Ele continua vendo, só para leitura, o que já enviou.`,
      confirmar: 'Desfazer vínculo',
      perigoso: true
    }).subscribe(ok => {
      if (ok) this.executar(v, this.service.desfazer(v.id), `Vínculo com ${v.nome} desfeito.`);
    });
  }

  private executar(v: VinculoVeterinario, acao: ReturnType<VeterinarioService['aprovar']>, mensagem: string) {
    this.processandoId = v.id;
    acao.subscribe({
      next: atualizado => {
        Object.assign(v, atualizado);
        this.processandoId = null;
        this.snack.open(mensagem, 'OK', { duration: 3000 });
      },
      error: err => {
        this.processandoId = null;
        this.snack.open(err?.error?.message ?? 'Não foi possível concluir.', 'OK', { duration: 4000 });
      }
    });
  }
}
