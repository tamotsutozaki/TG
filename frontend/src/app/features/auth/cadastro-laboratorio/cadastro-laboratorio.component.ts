import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators
} from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';
import { ConviteInfo } from '../../../core/models/auth.model';
import { UFS, normalizarCnpj } from '../../../shared/formatadores';

// CNPJ numérico ou alfanumérico: 12 caracteres + 2 dígitos (os dígitos são conferidos pela API)
function cnpjFormato(control: AbstractControl): ValidationErrors | null {
  const v = normalizarCnpj(control.value);
  return !v || /^[A-Z0-9]{12}\d{2}$/.test(v) ? null : { cnpj: true };
}

function senhasIguais(group: AbstractControl): ValidationErrors | null {
  return group.get('senha')?.value === group.get('confirmarSenha')?.value ? null : { senhasDiferentes: true };
}

@Component({
  selector: 'app-cadastro-laboratorio',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, RouterLink,
    MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule
  ],
  template: `
    <div class="cadastro-container">
      <mat-card class="cadastro-card">
        @if (validando) {
          <div class="estado"><mat-spinner diameter="40"></mat-spinner><p>Validando o convite...</p></div>
        } @else if (erroConvite) {
          <div class="estado erro">
            <mat-icon>link_off</mat-icon>
            <h2>Convite indisponível</h2>
            <p>{{ erroConvite }}</p>
            <p class="dica">Peça um novo convite ao administrador do sistema.</p>
            <a mat-stroked-button routerLink="/">Ir para o início</a>
          </div>
        } @else if (cadastrado) {
          <div class="estado sucesso">
            <mat-icon>verified</mat-icon>
            <h2>Laboratório cadastrado!</h2>
            <p>Entre com o e-mail <strong>{{ form.value.email }}</strong> e a senha que você criou.</p>
            <a mat-flat-button color="primary" routerLink="/login">Ir para o login</a>
          </div>
        } @else {
          <mat-card-header>
            <mat-card-title>Cadastro do laboratório</mat-card-title>
            <mat-card-subtitle>
              {{ convite?.descricao ? 'Convite: ' + convite?.descricao + ' · ' : '' }}válido até
              {{ convite?.expiraEm | date:'dd/MM/yyyy HH:mm' }}
            </mat-card-subtitle>
          </mat-card-header>

          <mat-card-content>
            <form [formGroup]="form" (ngSubmit)="enviar()" class="form">
              <h3>Laboratório</h3>
              <mat-form-field appearance="outline">
                <mat-label>Nome do laboratório</mat-label>
                <input matInput formControlName="nome" />
              </mat-form-field>
              <div class="linha">
                <mat-form-field appearance="outline" class="flex-2">
                  <mat-label>CNPJ</mat-label>
                  <input matInput formControlName="cnpj" placeholder="00.000.000/0000-00" maxlength="18" />
                  @if (form.get('cnpj')?.hasError('cnpj')) { <mat-error>CNPJ incompleto</mat-error> }
                </mat-form-field>
                <mat-form-field appearance="outline" class="flex-1">
                  <mat-label>Telefone</mat-label>
                  <input matInput formControlName="telefone" />
                </mat-form-field>
              </div>
              <mat-form-field appearance="outline">
                <mat-label>Endereço (rua, número e bairro)</mat-label>
                <input matInput formControlName="endereco" />
              </mat-form-field>
              <div class="linha">
                <mat-form-field appearance="outline" class="flex-2">
                  <mat-label>Cidade</mat-label>
                  <input matInput formControlName="cidade" />
                </mat-form-field>
                <mat-form-field appearance="outline" class="flex-1">
                  <mat-label>UF</mat-label>
                  <mat-select formControlName="uf">
                    @for (uf of ufs; track uf) { <mat-option [value]="uf">{{ uf }}</mat-option> }
                  </mat-select>
                </mat-form-field>
              </div>

              <h3>Responsável técnico</h3>
              <p class="ajuda">É quem assina os laudos emitidos pelo laboratório.</p>
              <mat-form-field appearance="outline">
                <mat-label>Nome completo</mat-label>
                <input matInput formControlName="responsavelTecnicoNome" />
              </mat-form-field>
              <div class="linha">
                <mat-form-field appearance="outline" class="flex-2">
                  <mat-label>Nº do CRMV</mat-label>
                  <input matInput formControlName="responsavelTecnicoCrmvNumero" inputmode="numeric" maxlength="6" />
                  @if (form.get('responsavelTecnicoCrmvNumero')?.hasError('pattern')) {
                    <mat-error>Apenas números (até 6 dígitos)</mat-error>
                  }
                </mat-form-field>
                <mat-form-field appearance="outline" class="flex-1">
                  <mat-label>UF do CRMV</mat-label>
                  <mat-select formControlName="responsavelTecnicoCrmvEstado">
                    @for (uf of ufs; track uf) { <mat-option [value]="uf">{{ uf }}</mat-option> }
                  </mat-select>
                </mat-form-field>
              </div>

              <h3>Acesso</h3>
              <p class="ajuda">Um único acesso para o laboratório. O e-mail também aparece nos laudos.</p>
              <mat-form-field appearance="outline">
                <mat-label>E-mail</mat-label>
                <input matInput type="email" formControlName="email" autocomplete="email" />
                @if (form.get('email')?.hasError('email')) { <mat-error>E-mail inválido</mat-error> }
              </mat-form-field>
              <div class="linha">
                <mat-form-field appearance="outline" class="flex-1">
                  <mat-label>Senha</mat-label>
                  <input matInput type="password" formControlName="senha" autocomplete="new-password" />
                  @if (form.get('senha')?.hasError('minlength')) { <mat-error>Mínimo de 8 caracteres</mat-error> }
                </mat-form-field>
                <mat-form-field appearance="outline" class="flex-1">
                  <mat-label>Confirmar senha</mat-label>
                  <input matInput type="password" formControlName="confirmarSenha" autocomplete="new-password" />
                </mat-form-field>
              </div>
              @if (form.hasError('senhasDiferentes') && form.get('confirmarSenha')?.touched) {
                <p class="erro-msg">As senhas não conferem.</p>
              }

              @if (erro) { <p class="erro-msg">{{ erro }}</p> }

              <button mat-flat-button color="primary" type="submit" [disabled]="enviando">
                @if (enviando) { <mat-spinner diameter="20"></mat-spinner> }
                @else { Cadastrar laboratório }
              </button>
            </form>
          </mat-card-content>
        }
      </mat-card>
    </div>
  `,
  styles: [`
    .cadastro-container {
      display: flex; justify-content: center; align-items: flex-start;
      min-height: 100vh; background: #f5f5f5; padding: 24px 16px;
    }
    .cadastro-card { width: 100%; max-width: 560px; padding: 8px; }
    .form { display: flex; flex-direction: column; gap: 4px; margin-top: 8px; }
    h3 { margin: 16px 0 4px; font-size: 15px; font-weight: 600; color: #3f51b5; }
    .ajuda { margin: 0 0 8px; font-size: 13px; color: #666; }
    .linha { display: flex; gap: 12px; }
    .flex-1 { flex: 1; min-width: 0; }
    .flex-2 { flex: 2; min-width: 0; }
    .estado { display: flex; flex-direction: column; align-items: center; text-align: center; padding: 32px 16px; gap: 8px; }
    .estado mat-icon { font-size: 56px; width: 56px; height: 56px; }
    .estado h2 { margin: 8px 0 0; }
    .estado p { margin: 0; color: #555; }
    .estado .dica { font-size: 13px; color: #888; margin-bottom: 12px; }
    .estado.erro mat-icon { color: #c62828; }
    .estado.sucesso mat-icon { color: #2e7d32; }
    .estado.sucesso a { margin-top: 12px; }
    @media (max-width: 480px) { .linha { flex-direction: column; gap: 0; } }
  `]
})
export class CadastroLaboratorioComponent implements OnInit {
  private auth = inject(AuthService);
  private route = inject(ActivatedRoute);

  ufs = UFS;
  token = '';
  convite: ConviteInfo | null = null;
  validando = true;
  erroConvite = '';
  enviando = false;
  cadastrado = false;
  erro = '';

  form = inject(FormBuilder).group({
    nome: ['', Validators.required],
    cnpj: ['', [Validators.required, cnpjFormato]],
    telefone: ['', Validators.required],
    endereco: ['', Validators.required],
    cidade: ['', Validators.required],
    uf: ['', Validators.required],
    responsavelTecnicoNome: ['', Validators.required],
    responsavelTecnicoCrmvNumero: ['', [Validators.required, Validators.pattern(/^\d{1,6}$/)]],
    responsavelTecnicoCrmvEstado: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    senha: ['', [Validators.required, Validators.minLength(8)]],
    confirmarSenha: ['', Validators.required]
  }, { validators: senhasIguais });

  ngOnInit() {
    this.token = this.route.snapshot.queryParamMap.get('convite') ?? '';
    if (!this.token) {
      this.validando = false;
      this.erroConvite = 'O link do convite está incompleto.';
      return;
    }

    this.auth.validarConvite(this.token).subscribe({
      next: c => { this.convite = c; this.validando = false; },
      error: err => {
        this.validando = false;
        this.erroConvite = err?.error?.message ?? 'Não foi possível validar o convite.';
      }
    });
  }

  enviar() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.enviando = true;
    this.erro = '';

    const { confirmarSenha, ...dados } = this.form.getRawValue();
    this.auth.registrarLaboratorio({ ...(dados as any), token: this.token }).subscribe({
      next: () => { this.enviando = false; this.cadastrado = true; },
      error: err => {
        this.enviando = false;
        this.erro = err?.error?.message ?? 'Não foi possível concluir o cadastro.';
      }
    });
  }
}
