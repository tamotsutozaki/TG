import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BreakpointObserver } from '@angular/cdk/layout';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
  private breakpoints = inject(BreakpointObserver);

  isMobile = toSignal(
    this.breakpoints.observe('(max-width: 768px)').pipe(map(r => r.matches)),
    { initialValue: false }
  );

  private readonly itensPatologista = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
    { label: 'Solicitações', icon: 'assignment', route: '/solicitacoes' },
    { label: 'Tipos de Exame', icon: 'science', route: '/tipos-exame' },
    { label: 'Estoque', icon: 'inventory_2', route: '/insumos' },
    { label: 'Veterinários', icon: 'groups', route: '/veterinarios' }
  ];

  private readonly itensVeterinario = [
    { label: 'Minhas Solicitações', icon: 'assignment', route: '/solicitacoes' }
  ];

  constructor(public auth: AuthService) {}

  get navItems() {
    return this.auth.isPatologista() ? this.itensPatologista : this.itensVeterinario;
  }

  fecharSeMobile(sidenav: { close: () => void }) {
    if (this.isMobile()) sidenav.close();
  }
}
