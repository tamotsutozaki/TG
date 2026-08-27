import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginInput } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'labpat_token';
  private readonly USER_KEY = 'labpat_user';

  isLoggedIn = signal(this.hasToken());
  currentUser = signal<{ nome: string; email: string } | null>(this.loadUser());

  constructor(private http: HttpClient, private router: Router) {}

  login(input: LoginInput) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, input).pipe(
      tap(res => {
        localStorage.setItem(this.TOKEN_KEY, res.token);
        localStorage.setItem(this.USER_KEY, JSON.stringify({ nome: res.nome, email: res.email }));
        this.isLoggedIn.set(true);
        this.currentUser.set({ nome: res.nome, email: res.email });
      })
    );
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

  private loadUser() {
    const raw = localStorage.getItem(this.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}
