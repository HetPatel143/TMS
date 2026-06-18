import { animate, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { format } from 'date-fns';
import { Subject, takeUntil } from 'rxjs';
import { DashboardLayoutComponent } from '../../../components/layout/dashboard-layout.component';
import { StatusBadgeComponent } from '../../../components/shared/status-badge.component';
import { ReportFilter, RequestResponse, RequestTypeResponse } from '../../../models';
import { DataService } from '../../../services/data.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-all-requests',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatSelectModule,
    MatTooltipModule,
    MatPaginatorModule,
    DashboardLayoutComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './all-requests.component.html',
  animations: [
    trigger('pageAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
  ],
})
export class AllRequestsComponent implements OnInit, OnDestroy {
  requests: RequestResponse[] = [];
  filteredRequests: RequestResponse[] = [];
  requestTypes: RequestTypeResponse[] = [];
  isLoading = true;
  filterForm: FormGroup;
  searchTerm = '';
  statuses = ['Draft', 'Submitted', 'InApproval', 'Approved', 'Rejected', 'Cancelled', 'Closed'];
  pageIndex = 0;
  pageSize = 10;
  private _allFiltered: RequestResponse[] = [];
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private dataService: DataService,
    private toast: ToastService
  ) {
    this.filterForm = this.fb.group({
      search: [''],
      status: [''],
      requestTypeId: [''],
    });
  }

  ngOnInit(): void {
    this.loadRequests();
    this.loadRequestTypes();
    this.filterForm.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.applyFilters());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadRequestTypes(): void {
    this.dataService.getAllRequestTypes()
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: (data) => (this.requestTypes = data) });
  }

  loadRequests(): void {
    this.isLoading = true;
    const filter: ReportFilter = {};
    const status = this.filterForm.value.status;
    if (status) filter.status = status;

    this.dataService.getReports(filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.requests = data;
          this.applyFilters();
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
          this.toast.error('Failed to load requests');
        },
      });
  }

 applyFilters(): void {
  const { search, status, requestTypeId } = this.filterForm.value;
  const all = this.requests.filter((r) => {
    const matchSearch = !search ||
      r.requestTypeName?.toLowerCase().includes(search.toLowerCase()) ||
      r.createdByName?.toLowerCase().includes(search.toLowerCase()) ||
      r.requestNumber?.toLowerCase().includes(search.toLowerCase()) ||
      String(r.requestId).includes(search);
    const matchStatus = !status || r.currentStatus === status;
    const matchType = !requestTypeId || r.requestTypeId === Number(requestTypeId);
    return matchSearch && matchStatus && matchType;
  });
  this.pageIndex = 0; // reset to first page on filter change
  this._allFiltered = all;
  this.filteredRequests = all.slice(0, this.pageSize);
}

get totalCount(): number { return this._allFiltered.length; }
get pendingCount(): number { return this._allFiltered.filter(r => r.currentStatus === 'InApproval').length; }
get approvedCount(): number { return this._allFiltered.filter(r => r.currentStatus === 'Approved').length; }
get rejectedCount(): number { return this._allFiltered.filter(r => r.currentStatus === 'Rejected').length; }

  getInitials(name: string): string {
    return name?.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2) ?? '?';
  }

  getAvatarColor(name: string): string {
    const colors = [
      'bg-blue-100 text-blue-700',
      'bg-purple-100 text-purple-700',
      'bg-green-100 text-green-700',
      'bg-orange-100 text-orange-700',
      'bg-pink-100 text-pink-700',
    ];
    const index = (name?.charCodeAt(0) ?? 0) % colors.length;
    return colors[index];
  }

  exportCsv(): void {
    const filter: ReportFilter = {};
    const status = this.filterForm.value.status;
    if (status) filter.status = status;

    this.dataService.exportReportsCsv(filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = `TMS_Report_${format(new Date(), 'yyyyMMdd_HHmmss')}.csv`;
          a.click();
          window.URL.revokeObjectURL(url);
          this.toast.success('Report exported successfully');
        },
        error: () => this.toast.error('Failed to export report'),
      });
  }


  onPageChange(event: PageEvent): void {
  this.pageIndex = event.pageIndex;
  this.pageSize = event.pageSize;
  const start = this.pageIndex * this.pageSize;
  this.filteredRequests = this._allFiltered.slice(start, start + this.pageSize);
}
formatDate(dateStr: string): string {
  return format(new Date(dateStr), 'MMM d, yyyy');
}
}
