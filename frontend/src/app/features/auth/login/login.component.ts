import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  form: FormGroup;
  loading = false;
  erro = '';
  senhaVisivel = false;
  // Página para onde voltar depois do login (ex.: link de vínculo de um laboratório)
  returnUrl: string | null;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    route: ActivatedRoute
  ) {
    const url = route.snapshot.queryParamMap.get('returnUrl');
    // Só endereços internos, para não redirecionar para outro site
    this.returnUrl = url && url.startsWith('/') && !url.startsWith('//') ? url : null;

    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      senha: ['', Validators.required]
    });
  }

  entrar() {
    if (this.form.invalid) return;

    this.loading = true;
    this.erro = '';

    this.auth.login(this.form.value).subscribe({
      next: () => {
        this.router.navigateByUrl(this.returnUrl ?? this.auth.rotaInicial());
      },
      error: err => {
        this.erro = err?.error?.message ?? 'E-mail ou senha inválidos.';
        this.loading = false;
      }
    });
  }
}
