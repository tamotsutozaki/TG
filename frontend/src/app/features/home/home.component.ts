import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="home-container">
      <div class="home-content">
        <header class="brand">
          <mat-icon class="brand-icon">biotech</mat-icon>
          <h1>LabPat</h1>
          <p>Laboratório de Patologia Veterinária</p>
        </header>

        <mat-card class="consulta-card">
          <mat-card-header>
            <mat-card-title>Consultar exame</mat-card-title>
            <mat-card-subtitle>Acompanhe o status do seu exame pelo código</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <form class="consulta-form" (ngSubmit)="consultar()">
              <mat-form-field appearance="outline" class="codigo-field">
                <mat-label>Código do exame</mat-label>
                <input matInput [(ngModel)]="codigo" name="codigo"
                       placeholder="Ex.: ABC123" (keyup.enter)="consultar()" />
                <mat-icon matPrefix>qr_code_2</mat-icon>
              </mat-form-field>
              <button mat-flat-button color="primary" type="submit"
                      [disabled]="!codigo.trim()">
                <mat-icon>search</mat-icon>
                Consultar
              </button>
            </form>
          </mat-card-content>
        </mat-card>

        <footer class="acesso-interno">
          @if (auth.isLoggedIn()) {
            <button mat-stroked-button (click)="irParaPainel()">
              <mat-icon>dashboard</mat-icon>
              Ir para o painel
            </button>
          } @else {
            <button mat-button (click)="irParaLogin()">
              <mat-icon>lock</mat-icon>
              Acesso do patologista
            </button>
          }
        </footer>
      </div>
    </div>
  `,
  styles: [`
    .home-container {
      display: flex; justify-content: center; align-items: center;
      min-height: 100vh; background: #f5f5f5; padding: 16px;
    }
    .home-content { width: 100%; max-width: 480px; }
    .brand { text-align: center; margin-bottom: 24px; color: #3f51b5; }
    .brand-icon { font-size: 48px; height: 48px; width: 48px; }
    .brand h1 { margin: 8px 0 4px; font-size: 32px; font-weight: 600; }
    .brand p { margin: 0; color: #666; font-size: 14px; }
    .consulta-card { padding: 8px; }
    .consulta-form { display: flex; flex-direction: column; gap: 8px; margin-top: 16px; }
    .codigo-field { width: 100%; }
    .acesso-interno { text-align: center; margin-top: 24px; }
  `]
})
export class HomeComponent {
  codigo = '';
  auth = inject(AuthService);
  private router = inject(Router);

  consultar() {
    const c = this.codigo.trim();
    if (!c) return;
    this.router.navigate(['/consulta', c]);
  }

  irParaLogin() {
    this.router.navigate(['/login']);
  }

  irParaPainel() {
    this.router.navigate(['/solicitacoes']);
  }
}
