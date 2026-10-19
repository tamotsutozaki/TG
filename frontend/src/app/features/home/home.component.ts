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

        <div class="acesso-vet">
          <mat-icon>pets</mat-icon>
          <div class="acesso-vet-texto">
            <span class="acesso-vet-titulo">É veterinário?</span>
            <span class="acesso-vet-sub">Envie solicitações de exame pela plataforma</span>
          </div>
          <button mat-stroked-button color="primary" (click)="irParaLogin()">Entrar</button>
          <button mat-flat-button color="primary" (click)="irParaRegistro()">Criar conta</button>
        </div>

        <footer class="acesso-interno">
          @if (auth.isLoggedIn()) {
            <button mat-button (click)="irParaPainel()">
              <mat-icon>dashboard</mat-icon>
              Ir para o painel
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
    .acesso-vet {
      display: flex; align-items: center; gap: 12px; flex-wrap: wrap;
      margin-top: 16px; padding: 16px; background: #fff; border-radius: 8px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }
    .acesso-vet > mat-icon { color: #3f51b5; }
    .acesso-vet-texto { display: flex; flex-direction: column; flex: 1; min-width: 140px; }
    .acesso-vet-titulo { font-weight: 600; font-size: 14px; }
    .acesso-vet-sub { font-size: 12px; color: #666; }
    .acesso-interno { text-align: center; margin-top: 16px; }
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

  irParaRegistro() {
    this.router.navigate(['/registro']);
  }

  irParaPainel() {
    this.router.navigate(['/dashboard']);
  }
}
