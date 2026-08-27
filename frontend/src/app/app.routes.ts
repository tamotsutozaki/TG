import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'solicitacoes', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent)
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
        loadComponent: () =>
          import('./features/tipos-exame/tipos-exame.component').then(m => m.TiposExameComponent)
      },
      {
        path: 'insumos',
        loadComponent: () =>
          import('./features/insumos/insumos.component').then(m => m.InsumosComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'solicitacoes' }
];
