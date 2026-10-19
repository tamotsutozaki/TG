import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { patologistaGuard } from './core/guards/patologista.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./features/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'registro',
    loadComponent: () =>
      import('./features/auth/registro/registro.component').then(m => m.RegistroComponent)
  },
  {
    path: 'consulta/:codigo',
    loadComponent: () =>
      import('./features/consulta-publica/consulta-publica.component').then(m => m.ConsultaPublicaComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layout/main-layout/main-layout.component').then(m => m.MainLayoutComponent),
    children: [
      {
        path: 'dashboard',
        canActivate: [patologistaGuard],
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'solicitacoes',
        loadComponent: () =>
          import('./features/solicitacoes/solicitacoes-list/solicitacoes-list.component').then(m => m.SolicitacoesListComponent)
      },
      {
        path: 'solicitacoes/nova',
        loadComponent: () =>
          import('./features/solicitacoes/nova-solicitacao/nova-solicitacao.component').then(m => m.NovaSolicitacaoComponent)
      },
      {
        path: 'solicitacoes/:id',
        loadComponent: () =>
          import('./features/solicitacoes/solicitacao-detalhe/solicitacao-detalhe.component').then(m => m.SolicitacaoDetalheComponent)
      },
      {
        path: 'tipos-exame',
        canActivate: [patologistaGuard],
        loadComponent: () =>
          import('./features/tipos-exame/tipos-exame.component').then(m => m.TiposExameComponent)
      },
      {
        path: 'insumos',
        canActivate: [patologistaGuard],
        loadComponent: () =>
          import('./features/insumos/insumos.component').then(m => m.InsumosComponent)
      },
      {
        path: 'veterinarios',
        canActivate: [patologistaGuard],
        loadComponent: () =>
          import('./features/veterinarios/veterinarios.component').then(m => m.VeterinariosComponent)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];
