import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { Observable } from 'rxjs';

export interface ConfirmarDialogData {
  titulo: string;
  mensagem: string;
  confirmar: string;
  perigoso?: boolean;
}

@Component({
  selector: 'app-confirmar-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ data.titulo }}</h2>
    <mat-dialog-content><p class="mensagem">{{ data.mensagem }}</p></mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button [mat-dialog-close]="false">Cancelar</button>
      <button mat-flat-button [color]="data.perigoso ? 'warn' : 'primary'" [mat-dialog-close]="true">
        {{ data.confirmar }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`.mensagem { margin: 0; max-width: min(420px, 80vw); line-height: 1.5; }`]
})
export class ConfirmarDialogComponent {
  data = inject<ConfirmarDialogData>(MAT_DIALOG_DATA);
}

/** Abre o diálogo de confirmação; emite true se a pessoa confirmar. */
export function confirmar(dialog: MatDialog, data: ConfirmarDialogData): Observable<boolean | undefined> {
  return dialog.open(ConfirmarDialogComponent, { data, autoFocus: false }).afterClosed();
}
