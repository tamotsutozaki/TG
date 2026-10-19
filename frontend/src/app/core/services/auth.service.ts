import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginInput, PapelUsuario, RegistrarVetInput } from '../models/auth.model';

interface UsuarioLogado {
  nome: string;
  email: string;
  papel: PapelUsuario;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'labpat_token';
  private readonly USER_KEY = 'labpat_user';

  isLoggedIn = signal(this.hasToken());
  currentUser = signal<UsuarioLogado | null>(this.loadUser());

  isPatologista = computed(() => this.currentUser()?.papel === 'Patologista');
  isVeterinario = computed(() => this.currentUser()?.papel === 'Veterinario');

  constructor(private http: HttpClient, private router: Router) {}

  login(input: LoginInput) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, input).pipe(
      tap(res => {
        localStorage.setItem(this.TOKEN_KEY, res.token);
        const user: UsuarioLogado = { nome: res.nome, email: res.email, papel: res.papel };
        localStorage.setItem(this.USER_KEY, JSON.stringify(user));
        this.isLoggedIn.set(true);
        this.currentUser.set(user);
      })
    );
  }

  registrar(input: RegistrarVetInput) {
    return this.http.post<{ message: string }>(`${environment.apiUrl}/auth/registrar`, input);
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

  private hasToken(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }

  private loadUser(): UsuarioLogado | null {
    const raw = localStorage.getItem(this.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}
