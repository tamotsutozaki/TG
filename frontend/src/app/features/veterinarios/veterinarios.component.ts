import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { VeterinarioService } from '../../core/services/veterinario.service';
import { VeterinarioConta } from '../../core/models/veterinario.model';

@Component({
  selector: 'app-veterinarios',
  standalone: true,
  imports: [
    CommonModule, MatCardModule, MatButtonModule, MatIconModule,
    MatProgressSpinnerModule, MatChipsModule, MatSnackBarModule
  ],
  templateUrl: './veterinarios.component.html',
  styleUrl: './veterinarios.component.scss'
})
export class VeterinariosComponent implements OnInit {
  private service = inject(VeterinarioService);
  private snack = inject(MatSnackBar);

  veterinarios: VeterinarioConta[] = [];
  loading = true;
  processandoId: number | null = null;

  statusLabels: Record<string, string> = {
    Pendente: 'Pendente',
    Aprovada: 'Aprovado',
    Recusada: 'Recusado'
  };

  ngOnInit() {
    this.carregar();
  }

  carregar() {
    this.loading = true;
    this.service.listar().subscribe({
      next: v => { this.veterinarios = v; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  get pendentes() {
    return this.veterinarios.filter(v => v.statusConta === 'Pendente');
  }

  get demais() {
    return this.veterinarios.filter(v => v.statusConta !== 'Pendente');
  }

  aprovar(v: VeterinarioConta) {
    this.processandoId = v.id;
    this.service.aprovar(v.id).subscribe({
      next: () => {
        v.statusConta = 'Aprovada';
        this.processandoId = null;
        this.snack.open(`${v.nome} aprovado.`, 'OK', { duration: 3000 });
      },
      error: () => { this.processandoId = null; }
    });
  }

  recusar(v: VeterinarioConta) {
    this.processandoId = v.id;
    this.service.recusar(v.id).subscribe({
      next: () => {
        v.statusConta = 'Recusada';
        this.processandoId = null;
        this.snack.open(`${v.nome} recusado.`, 'OK', { duration: 3000 });
      },
      error: () => { this.processandoId = null; }
    });
  }
}
