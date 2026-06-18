import { animate, style, transition, trigger } from '@angular/animations';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { NavItem } from '../../models';
import { AuthService } from '../../services/auth.service';
import { DataService } from '../../services/data.service';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatDividerModule,
    MatBadgeModule,
    MatProgressBarModule,
    MatTooltipModule,
  ],
  templateUrl: './dashboard-layout.component.html',
  styleUrl: './dashboard-layout.component.css',
  animations: [
    trigger('sidebarAnimation', [
      transition(':enter', [
        style({ transform: 'translateX(-100%)' }),
        animate('300ms ease-out', style({ transform: 'translateX(0)' })),
      ]),
    ]),
  ],
})
export class DashboardLayoutComponent implements OnInit, OnDestroy {
  isMobile = false;
  isDarkMode = false;
  sidenavOpened = true;
  pendingCount = 0;
  loading = false;
  private destroy$ = new Subject<void>();

  navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard', roles: ['Admin', 'Employee', 'Manager', 'Finance', 'IT', 'HR'] },
    { label: 'My Requests', icon: 'list_alt', route: '/requests', roles: ['Employee', 'Manager', 'Finance', 'IT', 'HR'] },
    { label: 'New Request', icon: 'add_circle', route: '/requests/new', roles: ['Employee', 'Manager', 'Finance', 'IT', 'HR'] },
    { label: 'Pending Approvals', icon: 'pending_actions', route: '/approvals', roles: ['Manager', 'Finance', 'IT', 'HR'] },
    { label: 'Manage Users', icon: 'people', route: '/admin/users', roles: ['Admin'] },
    { label: 'Request Types', icon: 'category', route: '/admin/request-types', roles: ['Admin'] },
    { label: 'Workflows', icon: 'account_tree', route: '/admin/workflows', roles: ['Admin'] },
    { label: 'All Requests', icon: 'assignment', route: '/admin/requests', roles: ['Admin'] },
  ];

  constructor(
    public authService: AuthService,
    private dataService: DataService,
    private breakpointObserver: BreakpointObserver,
    private router: Router
  ) {}

  ngOnInit(): void {
    const storedTheme = localStorage.getItem('tms_dark_mode');
    this.isDarkMode = storedTheme ? storedTheme === 'true' : document.documentElement.classList.contains('dark');
    document.documentElement.classList.toggle('dark', this.isDarkMode);
    this.breakpointObserver
      .observe([Breakpoints.Handset])
      .pipe(takeUntil(this.destroy$))
      .subscribe((result) => {
        this.isMobile = result.matches;
        this.sidenavOpened = !this.isMobile;
      });

    this.dataService.loading$
      .pipe(takeUntil(this.destroy$))
      .subscribe((loading) => (this.loading = loading));

    this.dataService.pendingApprovals$
      .pipe(takeUntil(this.destroy$))
      .subscribe((approvals) => (this.pendingCount = approvals.length));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get filteredNavItems(): NavItem[] {
    const role = this.authService.userRole;
    if (!role) return [];
    return this.navItems.filter((item) => item.roles.includes(role));
  }

  get userInitials(): string {
    const name = this.authService.currentUser?.fullName ?? '';
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }

  toggleSidenav(): void {
    this.sidenavOpened = !this.sidenavOpened;
  }

  toggleDarkMode(): void {
    this.isDarkMode = !this.isDarkMode;
    document.documentElement.classList.toggle('dark', this.isDarkMode);
    localStorage.setItem('tms_dark_mode', String(this.isDarkMode));
  }

  logout(): void {
    this.authService.logout();
  }

  onNavClick(): void {
    if (this.isMobile) {
      this.sidenavOpened = false;
    }
  }
}
