import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { papelGuard } from './core/guards/papel.guard';

export const routes: Routes = [
  // Páginas públicas
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
    // Aberta pelo link do convite gerado pelo administrador
    path: 'cadastro-laboratorio',
    loadComponent: () =>
      import('./features/auth/cadastro-laboratorio/cadastro-laboratorio.component').then(m => m.CadastroLaboratorioComponent)
  },
  {
    // Aberta pelo link que o laboratório envia aos veterinários
    path: 'vincular/:codigo',
    loadComponent: () =>
      import('./features/vinculos/vincular/vincular.component').then(m => m.VincularComponent)
  },
  {
    path: 'consulta/:codigo',
    loadComponent: () =>
      import('./features/consulta-publica/consulta-publica.component').then(m => m.ConsultaPublicaComponent)
  },

  // Área logada
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layout/main-layout/main-layout.component').then(m => m.MainLayoutComponent),
    children: [
      // Laboratório
      {
        path: 'dashboard',
        canActivate: [papelGuard('Patologista')],
        loadComponent: () =>
          import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'tipos-exame',
        canActivate: [papelGuard('Patologista')],
        loadComponent: () =>
          import('./features/tipos-exame/tipos-exame.component').then(m => m.TiposExameComponent)
      },
      {
        path: 'insumos',
        canActivate: [papelGuard('Patologista')],
        loadComponent: () =>
          import('./features/insumos/insumos.component').then(m => m.InsumosComponent)
      },
      {
        path: 'veterinarios',
        canActivate: [papelGuard('Patologista')],
        loadComponent: () =>
          import('./features/veterinarios/veterinarios.component').then(m => m.VeterinariosComponent)
      },

      // Laboratório e veterinário
      {
        path: 'solicitacoes',
        canActivate: [papelGuard('Patologista', 'Veterinario')],
        loadComponent: () =>
          import('./features/solicitacoes/solicitacoes-list/solicitacoes-list.component').then(m => m.SolicitacoesListComponent)
      },
      {
        path: 'solicitacoes/nova',
        canActivate: [papelGuard('Patologista', 'Veterinario')],
        loadComponent: () =>
          import('./features/solicitacoes/nova-solicitacao/nova-solicitacao.component').then(m => m.NovaSolicitacaoComponent)
      },
      {
        path: 'solicitacoes/:id',
        canActivate: [papelGuard('Patologista', 'Veterinario')],
        loadComponent: () =>
          import('./features/solicitacoes/solicitacao-detalhe/solicitacao-detalhe.component').then(m => m.SolicitacaoDetalheComponent)
      },

      // Veterinário
      {
        path: 'laboratorios',
        canActivate: [papelGuard('Veterinario')],
        loadComponent: () =>
          import('./features/vinculos/meus-laboratorios/meus-laboratorios.component').then(m => m.MeusLaboratoriosComponent)
      },

      // Administrador
      {
        path: 'admin/convites',
        canActivate: [papelGuard('Administrador')],
        loadComponent: () =>
          import('./features/admin/convites/convites.component').then(m => m.ConvitesComponent)
      },
      {
        path: 'admin/laboratorios',
        canActivate: [papelGuard('Administrador')],
        loadComponent: () =>
          import('./features/admin/laboratorios/laboratorios-admin.component').then(m => m.LaboratoriosAdminComponent)
      },
      {
        path: 'admin/veterinarios',
        canActivate: [papelGuard('Administrador')],
        loadComponent: () =>
          import('./features/admin/veterinarios/veterinarios-admin.component').then(m => m.VeterinariosAdminComponent)
      },

      // Todos os papéis
      {
        path: 'perfil',
        loadComponent: () =>
          import('./features/perfil/perfil.component').then(m => m.PerfilComponent)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];
