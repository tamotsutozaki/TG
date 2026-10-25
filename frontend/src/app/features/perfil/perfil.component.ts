import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Clipboard } from '@angular/cdk/clipboard';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../core/services/auth.service';
import { PerfilService } from '../../core/services/perfil.service';
import { PerfilLaboratorio, PerfilVeterinario } from '../../core/models/perfil.model';
import { UFS, formatarCrmv, linkAbsoluto } from '../../shared/formatadores';

function senhasIguais(group: AbstractControl): ValidationErrors | null {
  return group.get('novaSenha')?.value === group.get('confirmarSenha')?.value ? null : { senhasDiferentes: true };
}

@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatSnackBarModule
  ],
  templateUrl: './perfil.component.html',
  styleUrl: './perfil.component.scss'
})
export class PerfilComponent implements OnInit {
  auth = inject(AuthService);
  private service = inject(PerfilService);
  private clipboard = inject(Clipboard);
  private snack = inject(MatSnackBar);
  private fb = inject(FormBuilder);

  ufs = UFS;
  loading = true;
  salvando = false;
  trocandoSenha = false;

  laboratorio: PerfilLaboratorio | null = null;
  veterinario: PerfilVeterinario | null = null;

  formLaboratorio = this.fb.group({
    nome: ['', Validators.required],
    endereco: ['', Validators.required],
    cidade: ['', Validators.required],
    uf: ['', Validators.required],
    telefone: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    responsavelTecnicoNome: ['', Validators.required],
    responsavelTecnicoCrmvNumero: ['', [Validators.required, Validators.pattern(/^\d{1,6}$/)]],
    responsavelTecnicoCrmvEstado: ['', Validators.required]
  });

  formVeterinario = this.fb.group({
    nome: ['', Validators.required],
    telefone: ['']
  });

  formSenha = this.fb.group({
    senhaAtual: ['', Validators.required],
    novaSenha: ['', [Validators.required, Validators.minLength(8)]],
    confirmarSenha: ['', Validators.required]
  }, { validators: senhasIguais });

  ngOnInit() {
    if (this.auth.isPatologista()) {
      this.service.getLaboratorio().subscribe({
        next: p => this.carregarLaboratorio(p),
        error: () => { this.loading = false; }
      });
    } else if (this.auth.isVeterinario()) {
      this.service.getVeterinario().subscribe({
        next: p => this.carregarVeterinario(p),
        error: () => { this.loading = false; }
      });
    } else {
      this.loading = false;
    }
  }

  get linkVinculo(): string {
    return this.laboratorio ? linkAbsoluto(`/vincular/${this.laboratorio.codigoVinculo}`) : '';
  }

  get crmvVeterinario(): string {
    return this.veterinario ? formatarCrmv(this.veterinario.crmvEstado, this.veterinario.crmvNumero) : '';
  }

  copiarLink() {
    if (this.clipboard.copy(this.linkVinculo)) {
      this.snack.open('Link copiado! Envie aos veterinários.', 'OK', { duration: 3000 });
    }
  }

  salvarLaboratorio() {
    if (this.formLaboratorio.invalid) { this.formLaboratorio.markAllAsTouched(); return; }
    this.salvando = true;
    this.service.atualizarLaboratorio(this.formLaboratorio.getRawValue() as any).subscribe({
      next: p => {
        this.carregarLaboratorio(p);
        this.auth.atualizarNome(p.nome);
        this.salvando = false;
        this.snack.open('Dados do laboratório atualizados.', 'OK', { duration: 3000 });
      },
      error: err => this.falhaAoSalvar(err)
    });
  }

  salvarVeterinario() {
    if (this.formVeterinario.invalid) { this.formVeterinario.markAllAsTouched(); return; }
    this.salvando = true;
    const { nome, telefone } = this.formVeterinario.getRawValue();
    this.service.atualizarVeterinario({ nome: nome!, telefone: telefone || null }).subscribe({
      next: p => {
        this.carregarVeterinario(p);
        this.auth.atualizarNome(p.nome);
        this.salvando = false;
        this.snack.open('Seus dados foram atualizados.', 'OK', { duration: 3000 });
      },
      error: err => this.falhaAoSalvar(err)
    });
  }

  alterarSenha() {
    if (this.formSenha.invalid) { this.formSenha.markAllAsTouched(); return; }
    this.trocandoSenha = true;
    const { senhaAtual, novaSenha } = this.formSenha.getRawValue();
    this.service.alterarSenha(senhaAtual!, novaSenha!).subscribe({
      next: () => {
        this.trocandoSenha = false;
        this.formSenha.reset();
        this.snack.open('Senha alterada.', 'OK', { duration: 3000 });
      },
      error: err => {
        this.trocandoSenha = false;
        this.snack.open(err?.error?.message ?? 'Não foi possível alterar a senha.', 'OK', { duration: 4000 });
      }
    });
  }

  private carregarLaboratorio(p: PerfilLaboratorio) {
    this.laboratorio = p;
    this.formLaboratorio.reset({
      nome: p.nome, endereco: p.endereco, cidade: p.cidade, uf: p.uf, telefone: p.telefone,
      email: p.email, responsavelTecnicoNome: p.responsavelTecnicoNome,
      responsavelTecnicoCrmvNumero: p.responsavelTecnicoCrmvNumero,
      responsavelTecnicoCrmvEstado: p.responsavelTecnicoCrmvEstado
    });
    this.loading = false;
  }

  private carregarVeterinario(p: PerfilVeterinario) {
    this.veterinario = p;
    this.formVeterinario.reset({ nome: p.nome, telefone: p.telefone ?? '' });
    this.loading = false;
  }

  private falhaAoSalvar(err: any) {
    this.salvando = false;
    this.snack.open(err?.error?.message ?? 'Não foi possível salvar.', 'OK', { duration: 4000 });
  }
}
