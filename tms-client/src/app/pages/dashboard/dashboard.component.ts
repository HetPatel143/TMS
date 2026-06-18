import { animate, query, stagger, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';
import { format } from 'date-fns';
import { Subject, takeUntil } from 'rxjs';
import { DashboardLayoutComponent } from '../../components/layout/dashboard-layout.component';
import {
  AdminDashboard,
  ApproverDashboard,
  EmployeeDashboard,
  RoleName,
} from '../../models';
import { AuthService } from '../../services/auth.service';
import { DataService } from '../../services/data.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatChipsModule,
    DashboardLayoutComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  animations: [
    trigger('pageAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
    trigger('staggerCards', [
      transition(':enter', [
        query('.stat-card', [
          style({ opacity: 0, transform: 'translateY(20px)' }),
          stagger(80, [
            animate('400ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
          ]),
        ], { optional: true }),
      ]),
    ]),
  ],
})
export class DashboardComponent implements OnInit, OnDestroy {
  role: RoleName | null = null;
  employeeData: EmployeeDashboard | null = null;
  approverData: ApproverDashboard | null = null;
  adminData: AdminDashboard | null = null;
  isLoading = true;
  draftRequests: any[] = [];
  private destroy$ = new Subject<void>();

  recentColumns = ['requestId', 'requestTypeName', 'currentStatus', 'createdAt'];
  pendingColumns = ['requestId', 'requestTypeName', 'requesterName', 'submittedAt'];
  adminRecentColumns = ['requestTypeName', 'totalRequests', 'avgApprovalDurationHours'];

  constructor(
    public authService: AuthService,
    private dataService: DataService
  ) {}

  ngOnInit(): void {
    this.role = this.authService.userRole;
    this.loadDashboard();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadDashboard(): void {
    this.isLoading = true;

    if (this.role === 'Employee') {
      this.dataService.getEmployeeDashboard()
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (data) => {
            this.employeeData = data;
            this.draftRequests = (data.recentRequests || [])
          .filter(r => r.currentStatus === 'Draft');
            this.isLoading = false;
          },
          error: () => (this.isLoading = false),
        });
    } else if (this.role === 'Admin') {
      this.dataService.getAdminDashboard()
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (data) => {
            this.adminData = data;
            this.isLoading = false;
          },
          error: () => (this.isLoading = false),
        });
    } else {
      // Approver roles: Manager, Finance, IT, HR
      this.dataService.getApproverDashboard()
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (data) => {
            this.approverData = data;
            this.isLoading = false;
          },
          error: () => (this.isLoading = false),
        });
      // Also load pending approvals for badge count
      this.dataService.getPendingApprovals()
        .pipe(takeUntil(this.destroy$))
        .subscribe();
    }
  }

  formatDate(dateStr: string): string {
    return format(new Date(dateStr), 'MMM dd, yyyy');
  }

  get statusEntries(): { key: string; value: number }[] {
    if (!this.adminData?.requestsByStatus) return [];
    return Object.entries(this.adminData.requestsByStatus).map(([key, value]) => ({ key, value }));
  }

  getStatusIcon(status: string): string {
    const iconMap: Record<string, string> = {
      Draft: 'edit_note',
      Submitted: 'send',
      InApproval: 'hourglass_top',
      Approved: 'check_circle',
      Rejected: 'cancel',
      Cancelled: 'block',
      Closed: 'lock',
    };
    return iconMap[status] ?? 'info';
  }

  getStatusColor(status: string): string {
    const colorMap: Record<string, string> = {
      Draft: 'text-slate-500',
      Submitted: 'text-blue-500',
      InApproval: 'text-amber-500',
      Approved: 'text-emerald-500',
      Rejected: 'text-red-500',
      Cancelled: 'text-slate-400',
      Closed: 'text-indigo-500',
    };
    return colorMap[status] ?? 'text-slate-500';
  }

  get adminApprovedCount(): number {
  return this.adminData?.requestsByStatus?.['Approved'] ?? 0;
}

get adminPendingCount(): number {
  return this.adminData?.pendingApprovalsCount ?? 0;
}

get adminStatusEntries(): { key: string; value: number }[] {
  if (!this.adminData?.requestsByStatus) return [];
  return Object.entries(this.adminData.requestsByStatus).map(([key, value]) => ({ key, value }));
}
  
}
