import { animate, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { format } from 'date-fns';
import { Subject, takeUntil } from 'rxjs';
import { DashboardLayoutComponent } from '../../components/layout/dashboard-layout.component';
import { StatusBadgeComponent } from '../../components/shared/status-badge.component';
import { RequestResponse } from '../../models';
import { AuthService } from '../../services/auth.service';
import { DataService } from '../../services/data.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-request-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
    MatTableModule,
    MatTooltipModule,
    DashboardLayoutComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './request-detail.component.html',
  animations: [
    trigger('pageAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
  ],
})
export class RequestDetailComponent implements OnInit, OnDestroy {
  request: RequestResponse | null = null;
  isLoading = true;
  historyColumns = ['oldStatus', 'newStatus', 'changedByName', 'changeReason', 'changedAt'];
  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private dataService: DataService,
    private toast: ToastService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadRequest(id);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadRequest(id: number): void {
    this.isLoading = true;
    this.dataService.getRequestById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.request = data;
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
          this.toast.error('Failed to load request details');
          this.router.navigate(['/requests']);
        },
      });
  }

  submitRequest(): void {
  if (!this.request) return;
  this.dataService.submitRequest(this.request.requestId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: () => {
        this.toast.success('Request submitted for approval');
        this.loadRequest(this.request!.requestId); // reload to show updated status
      },
      error: () => this.toast.error('Failed to submit request'),
    });
}

 cancelRequest(): void {
  if (!this.request) return;
  if (confirm('Are you sure you want to cancel this request?')) {
    this.dataService.cancelRequest(this.request.requestId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toast.success('Request cancelled successfully');
          this.router.navigate(['/requests']); // navigate away instead of reloading
        },
        error: (err) => this.toast.error(err.error?.message || 'Failed to cancel request'),
      });
  }
}
  formatDate(dateStr: string | null): string {
    if (!dateStr) return '—';
    return format(new Date(dateStr), 'MMM dd, yyyy HH:mm');
  }

  get canSubmit(): boolean {
    return this.request?.currentStatus === 'Draft' &&
           this.request?.createdByUserId === this.authService.currentUser?.userId;
  }

 get canCancel(): boolean {
  if (!this.request) return false;
  if (this.request.createdByUserId !== this.authService.currentUser?.userId) return false;
  const status = this.request.currentStatus;
  if (status === 'Draft' || status === 'Submitted') return true;
  // InApproval — allow cancel regardless of approval progress
  if (status === 'InApproval') return true;
  return false;
}
get canEdit(): boolean {
  if (!this.request) return false;
  if (this.request.createdByUserId !== this.authService.currentUser?.userId) return false;
  if (this.request.currentStatus === 'Draft') return true;
  // InApproval — allow edit only if NO approval has been acted on yet
  if (this.request.currentStatus === 'InApproval') {
    const anyActed = this.request.approvals.some(
      a => a.approvalStatus === 'Approved' || a.approvalStatus === 'Rejected'
    );
    return !anyActed;
  }
  return false;
}

  getApprovalStatusIcon(status: string): string {
    switch (status) {
      case 'Approved': return 'check_circle';
      case 'Rejected': return 'cancel';
      default: return 'schedule';
    }
  }

  getApprovalStatusColor(status: string): string {
    switch (status) {
      case 'Approved': return 'text-emerald-500';
      case 'Rejected': return 'text-red-500';
      default: return 'text-amber-500';
    }
  }
  // Long date format: "February 1st, 2024"
formatDateLong(dateStr: string | null): string {
  if (!dateStr) return '—';
  return format(new Date(dateStr), 'MMMM do, yyyy');
}

getStatusIcon(status: string): string {
  const iconMap: Record<string, string> = {
    Draft:      'edit_note',
    Submitted:  'send',
    InApproval: 'schedule',
    Approved:   'check_circle',
    Rejected:   'cancel',
    Cancelled:  'block',
    Closed:     'lock',
  };
  return iconMap[status] ?? 'info';
}

getRoleIcon(role: string): string {
  const iconMap: Record<string, string> = {
    Manager: 'person',
    IT:      'computer',
    Finance: 'account_balance',
    HR:      'groups',
  };
  return iconMap[role] ?? 'badge';
}
}
