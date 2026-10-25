import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { VinculoService } from '../../../core/services/vinculo.service';
import {
  LaboratorioBusca, MeuVinculo, STATUS_VINCULO_LABEL, StatusVinculo
} from '../../../core/models/vinculo.model';

@Component({
  selector: 'app-meus-laboratorios',
  standalone: true,
  imports: [
    CommonModule, FormsModule, RouterLink,
    MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule,
    MatIconModule, MatProgressSpinnerModule, MatSnackBarModule
  ],
  templateUrl: './meus-laboratorios.component.html',
  styleUrl: './meus-laboratorios.component.scss'
})
export class MeusLaboratoriosComponent implements OnInit {
  private service = inject(VinculoService);
  private snack = inject(MatSnackBar);

  vinculos: MeuVinculo[] = [];
  resultados: LaboratorioBusca[] = [];
  termo = '';
  carregandoVinculos = true;
  buscando = false;
  enviandoId: number | null = null;

  ngOnInit() {
    this.carregarVinculos();
    this.buscar();
  }

  carregarVinculos() {
    this.service.meusVinculos().subscribe({
      next: v => { this.vinculos = v; this.carregandoVinculos = false; },
      error: () => { this.carregandoVinculos = false; }
    });
  }

  buscar() {
    this.buscando = true;
    this.service.buscarLaboratorios(this.termo).subscribe({
      next: r => { this.resultados = r; this.buscando = false; },
      error: () => { this.buscando = false; }
    });
  }

  podePedir(status?: StatusVinculo | null): boolean {
    return !status || status === 'Recusado' || status === 'Desfeito';
  }

  pedir(laboratorioId: number) {
    this.enviandoId = laboratorioId;
    this.service.solicitar(laboratorioId).subscribe({
      next: v => {
        this.enviandoId = null;
        const lab = this.resultados.find(r => r.id === laboratorioId);
        if (lab) lab.statusVinculo = v.status;
        this.carregarVinculos();
        this.snack.open(`Pedido enviado para ${v.laboratorioNome}.`, 'OK', { duration: 3000 });
      },
      error: err => {
        this.enviandoId = null;
        this.snack.open(err?.error?.message ?? 'Não foi possível enviar o pedido.', 'OK', { duration: 4000 });
      }
    });
  }

  label(status: StatusVinculo) { return STATUS_VINCULO_LABEL[status]; }
}
