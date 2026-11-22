import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';
import { UFS } from '../../../shared/formatadores';
import { LIMITE, MASCARAS, crmv, email, erroCampo, obrigatorio, soLetras, telefone } from '../../../shared/validacao';

@Component({
  selector: 'app-registro',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterLink,
    MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule, ...MASCARAS
  ],
  template: `
    <div class="registro-container">
      <mat-card class="registro-card">
        <a mat-button routerLink="/" class="voltar-link">
          <mat-icon>arrow_back</mat-icon> Voltar
        </a>

        @if (enviado) {
          <div class="sucesso">
            <mat-icon class="sucesso-icon">how_to_reg</mat-icon>
            <h2>Conta criada!</h2>
            <p>Entre no sistema e peça vínculo aos laboratórios com que você trabalha.
               Cada laboratório aprova o próprio vínculo.</p>
            <a mat-flat-button color="primary" routerLink="/login"
               [queryParams]="returnUrl ? { returnUrl } : {}">Ir para o login</a>
          </div>
        } @else {
          <mat-card-header>
            <mat-card-title>Criar conta de veterinário</mat-card-title>
            <mat-card-subtitle>Para enviar solicitações de exame aos laboratórios</mat-card-subtitle>
          </mat-card-header>

          <mat-card-content>
            <form [formGroup]="form" (ngSubmit)="enviar()" class="form" novalidate>
              <mat-form-field appearance="outline">
                <mat-label>Nome completo</mat-label>
                <input matInput formControlName="nome" [maxlength]="limite.nome" />
                <mat-error>{{ erro(form.get('nome')) }}</mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>E-mail</mat-label>
                <input matInput type="email" formControlName="email" [maxlength]="limite.email" />
                <mat-error>{{ erro(form.get('email')) }}</mat-error>
              </mat-form-field>

              <mat-form-field appearance="outline">
                <mat-label>Senha</mat-label>
                <input matInput type="password" formControlName="senha" [maxlength]="limite.senha" />
                <mat-error>{{ erro(form.get('senha')) }}</mat-error>
              </mat-form-field>

              <div class="crmv-row">
                <mat-form-field appearance="outline" class="crmv-numero">
                  <mat-label>Nº do CRMV</mat-label>
                  <input matInput formControlName="crmvNumero" appSoDigitos inputmode="numeric" maxlength="6" />
                  <mat-error>{{ erro(form.get('crmvNumero')) }}</mat-error>
                </mat-form-field>

                <mat-form-field appearance="outline" class="crmv-uf">
                  <mat-label>UF</mat-label>
                  <mat-select formControlName="crmvEstado">
                    @for (uf of ufs; track uf) {
                      <mat-option [value]="uf">{{ uf }}</mat-option>
                    }
                  </mat-select>
                  <mat-error>{{ erro(form.get('crmvEstado')) }}</mat-error>
                </mat-form-field>
              </div>

              <mat-form-field appearance="outline">
                <mat-label>Telefone (opcional)</mat-label>
                <input matInput formControlName="telefone" appTelefone inputmode="tel" placeholder="(19) 99999-1111" maxlength="15" />
                <mat-error>{{ erro(form.get('telefone')) }}</mat-error>
              </mat-form-field>

              @if (erroApi) { <p class="erro">{{ erroApi }}</p> }

              <button mat-flat-button color="primary" type="submit"
                      [disabled]="form.invalid || enviando">
                @if (enviando) { <mat-spinner diameter="20"></mat-spinner> }
                @else { Criar conta }
              </button>

              <p class="login-hint">
                Já tem conta? <a routerLink="/login" [queryParams]="returnUrl ? { returnUrl } : {}">Entrar</a>
              </p>
            </form>
          </mat-card-content>
        }
      </mat-card>
    </div>
  `,
  styles: [`
    .registro-container {
      display: flex; justify-content: center; align-items: flex-start;
      min-height: 100vh; background: #f5f5f5; padding: 24px 16px;
    }
    .registro-card { width: 100%; max-width: 480px; padding: 8px; }
    .voltar-link { margin: 4px 0 8px; }
    .form { display: flex; flex-direction: column; gap: 4px; margin-top: 8px; }
    .crmv-row { display: flex; gap: 12px; }
    .crmv-numero { flex: 2; }
    .crmv-uf { flex: 1; }
    .erro { color: #c62828; font-size: 14px; margin: 4px 0; }
    .login-hint { text-align: center; font-size: 14px; margin-top: 12px; color: #666; }
    .sucesso { text-align: center; padding: 24px 16px; }
    .sucesso-icon { font-size: 56px; height: 56px; width: 56px; color: #2e7d32; }
    .sucesso h2 { margin: 12px 0 8px; }
    .sucesso p { color: #666; margin-bottom: 20px; }
  `]
})
export class RegistroComponent {
  ufs = UFS;
  form: FormGroup;
  enviando = false;
  enviado = false;
  erroApi = '';
  erro = erroCampo;
  limite = LIMITE;

  private fb = inject(FormBuilder);
  private auth = inject(AuthService);

  // Volta para o link de vínculo, se o cadastro começou por ele
  returnUrl = inject(ActivatedRoute).snapshot.queryParamMap.get('returnUrl');

  constructor() {
    this.form = this.fb.group({
      nome: ['', [obrigatorio, soLetras, Validators.maxLength(LIMITE.nome)]],
      email: ['', [obrigatorio, email, Validators.maxLength(LIMITE.email)]],
      senha: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(LIMITE.senha)]],
      crmvNumero: ['', [obrigatorio, crmv]],
      crmvEstado: ['', Validators.required],
      telefone: ['', telefone]
    });
  }

  enviar() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.enviando = true;
    this.erroApi = '';
    this.auth.registrar(this.form.value).subscribe({
      next: () => { this.enviando = false; this.enviado = true; },
      error: err => {
        this.enviando = false;
        this.erroApi = err?.error?.message ?? 'Não foi possível concluir o cadastro.';
      }
    });
  }
}
