import { animate, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { format } from 'date-fns';
import { Subject, takeUntil } from 'rxjs';
import { DashboardLayoutComponent } from '../../components/layout/dashboard-layout.component';
import { StatusBadgeComponent } from '../../components/shared/status-badge.component';
import { ApprovalHistoryItem, RequestTypeResponse } from '../../models';
import { DataService } from '../../services/data.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-pending-approvals',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatTableModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    DashboardLayoutComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './pending-approvals.component.html',
  animations: [
    trigger('pageAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
  ],
})
export class PendingApprovalsComponent implements OnInit, OnDestroy {
  displayedColumns = ['requestId', 'requestTypeName', 'requesterName', 'approvalStatus', 'createdAt', 'actions'];
  approvals: ApprovalHistoryItem[] = [];
  requestTypes: string[] = [];
  allRequestTypes: RequestTypeResponse[] = [];
  filterForm: FormGroup;
  isLoading = true;
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private dataService: DataService,
    private toast: ToastService,
  ) {
    this.filterForm = this.fb.group({
      search: [''],
      requestType: [''],
    });
  }

  get searchControl(): FormControl {
    return this.filterForm.get('search') as FormControl;
  }

  get requestTypeControl(): FormControl {
    return this.filterForm.get('requestType') as FormControl;
  }

  ngOnInit(): void {
    this.loadApprovals();
    this.dataService.getAllRequestTypes()
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (types) => this.allRequestTypes = types.filter(t => t.isActive),
      error: () => {}
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadApprovals(): void {
    this.isLoading = true;
    this.dataService.getPendingApprovals()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.approvals = data;
          // Extract unique request types dynamically from real data
          this.requestTypes = [...new Set(data.map(item => item.requestTypeName))];
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
          this.toast.error('Failed to load pending approvals');
        },
      });
  }

  quickApprove(requestId: number, event: Event): void {
    event.stopPropagation();
    const comment = prompt('Add a comment (optional):');
    this.dataService.approveRequest(requestId, comment)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => { this.toast.success('Request approved'); this.loadApprovals(); },
        error: (err) => this.toast.error(err.error?.message || 'Failed to approve'),
      });
  }

  quickReject(requestId: number, event: Event): void {
    event.stopPropagation();
    const comment = prompt('Reason for rejection (required):');
    if (!comment || comment.trim() === '') {
      this.toast.error('Comment is mandatory when rejecting a request');
      return;
    }
    this.dataService.rejectRequest(requestId, comment)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => { this.toast.success('Request rejected'); this.loadApprovals(); },
        error: (err) => this.toast.error(err.error?.message || 'Failed to reject'),
      });
  }

  formatDate(dateStr: string): string {
    return format(new Date(dateStr), 'MMM dd, yyyy');
  }

  get visibleApprovals(): ApprovalHistoryItem[] {
    const search = (this.filterForm.value.search || '').toLowerCase().trim();
    const type = (this.filterForm.value.requestType || '').toLowerCase().trim();
    return this.approvals.filter(item => {
      const matchesSearch = !search ||
        item.requestTypeName.toLowerCase().includes(search) ||
        item.requesterName.toLowerCase().includes(search) ||
        item.requestNumber.toLowerCase().includes(search) ||
        item.requestId.toString().includes(search);
      const matchesType = !type || item.requestTypeName.toLowerCase() === type;
      return matchesSearch && matchesType;
    });
  }
}
