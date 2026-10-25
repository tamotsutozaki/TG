import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthResponse, ConviteInfo, LoginInput, PapelUsuario,
  RegistrarLaboratorioInput, RegistrarVetInput
} from '../models/auth.model';

interface UsuarioLogado {
  nome: string;
  email: string;
  papel: PapelUsuario;
}

// Página inicial de cada papel depois do login
const ROTA_INICIAL: Record<PapelUsuario, string> = {
  Patologista: '/dashboard',
  Veterinario: '/solicitacoes',
  Administrador: '/admin/convites'
};

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'labpat_token';
  private readonly USER_KEY = 'labpat_user';

  isLoggedIn = signal(this.hasToken());
  currentUser = signal<UsuarioLogado | null>(this.loadUser());

  isPatologista = computed(() => this.currentUser()?.papel === 'Patologista');
  isVeterinario = computed(() => this.currentUser()?.papel === 'Veterinario');
  isAdministrador = computed(() => this.currentUser()?.papel === 'Administrador');

  constructor(private http: HttpClient, private router: Router) {}

  login(input: LoginInput) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, input).pipe(
      tap(res => {
        localStorage.setItem(this.TOKEN_KEY, res.token);
        this.salvarUsuario({ nome: res.nome, email: res.email, papel: res.papel });
        this.isLoggedIn.set(true);
      })
    );
  }

  registrar(input: RegistrarVetInput) {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/auth/registrar`, input);
  }

  validarConvite(token: string) {
    return this.http.post<ConviteInfo>(`${environment.apiUrl}/auth/convites/validar`, { token });
  }

  registrarLaboratorio(input: RegistrarLaboratorioInput) {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/auth/registrar-laboratorio`, input);
  }

  rotaInicial(): string {
    const papel = this.currentUser()?.papel;
    return papel ? ROTA_INICIAL[papel] : '/login';
  }

  // Mantém o nome exibido no menu em dia (ex.: laboratório renomeado no "Meu perfil")
  atualizarNome(nome: string) {
    const atual = this.currentUser();
    if (atual) this.salvarUsuario({ ...atual, nome });
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.isLoggedIn.set(false);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private salvarUsuario(user: UsuarioLogado) {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUser.set(user);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }

  private loadUser(): UsuarioLogado | null {
    const raw = localStorage.getItem(this.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}
