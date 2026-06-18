import { animate, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { format } from 'date-fns';
import { merge, Subject, takeUntil } from 'rxjs';
import { DashboardLayoutComponent } from '../../components/layout/dashboard-layout.component';
import { RequestFilter, RequestResponse } from '../../models';
import { DataService } from '../../services/data.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-my-requests',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    DashboardLayoutComponent,
  ],
  templateUrl: './my-requests.component.html',
  animations: [
    trigger('pageAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
  ],
})
export class MyRequestsComponent implements OnInit, OnDestroy {
  displayedColumns = ['requestId', 'requestTypeName', 'currentStatus', 'createdAt', 'updatedAt', 'actions'];
  requests: RequestResponse[] = [];
  totalCount = 0;
  isLoading = true;
  filterForm: FormGroup;
  statuses = ['Draft', 'Submitted', 'InApproval', 'Approved', 'Rejected', 'Cancelled', 'Closed'];
  requestTypes: { requestTypeId: number; name: string }[] = [];
  
  private destroy$ = new Subject<void>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private fb: FormBuilder,
    private dataService: DataService,
    private toast: ToastService
  ) {
    this.filterForm = this.fb.group({
      status: [''],
      requestTypeId: [''],
      search: [''],
      fromDate: [''],
      toDate: [''],
    });
  }

  ngOnInit(): void {
    this.loadRequests();
    merge(
      this.filterForm.get('status')!.valueChanges,
      this.filterForm.get('requestTypeId')!.valueChanges,
      this.filterForm.get('fromDate')!.valueChanges,
      this.filterForm.get('toDate')!.valueChanges
    )
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.loadRequests());
        this.dataService.getAllRequestTypes()
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (types) => this.requestTypes = types.filter(t => t.isActive),
      error: () => {}
    });
  }

  
  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadRequests(pageEvent?: PageEvent): void {
    this.isLoading = true;
    const filter: RequestFilter = {
      pageNumber: pageEvent ? pageEvent.pageIndex + 1 : 1,
      pageSize: pageEvent ? pageEvent.pageSize : 10,
      status: this.filterForm.value.status || undefined,
      requestTypeId: this.filterForm.value.requestTypeId || undefined,
      fromDate: this.filterForm.value.fromDate || undefined,
      toDate: this.filterForm.value.toDate || undefined,
    };

    this.dataService.getMyRequests(filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.requests = result.items;
          this.totalCount = result.totalCount;
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
          this.toast.error('Failed to load requests');
        },
      });
  }

  cancelRequest(id: number, event: Event): void {
    event.stopPropagation();
    if (confirm('Are you sure you want to cancel this request?')) {
      this.dataService.cancelRequest(id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.toast.success('Request cancelled successfully');
            this.loadRequests();
          },
          error: () => this.toast.error('Failed to cancel request'),
        });
    }
  }

  formatDate(dateStr: string): string {
    return format(new Date(dateStr), 'MMM dd, yyyy');
  }

  clearFilters(): void {
    this.filterForm.reset({ status: '', requestTypeId: '', search: '', fromDate: '', toDate: '' });
  }

  get visibleRequests(): RequestResponse[] {
    const search = (this.filterForm.value.search || '').toLowerCase().trim();
    if (!search) return this.requests;
    return this.requests.filter((r) =>
      r.requestTypeName.toLowerCase().includes(search) ||
      r.requestNumber.toLowerCase().includes(search) ||
      String(r.requestId).includes(search)
    );
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
}
