import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { trigger, transition, style, animate } from '@angular/animations';
import { Subject, takeUntil } from 'rxjs';
import { format } from 'date-fns';
import { DashboardLayoutComponent } from '../../components/layout/dashboard-layout.component';
import { StatusBadgeComponent } from '../../components/shared/status-badge.component';
import { DataService } from '../../services/data.service';
import { ToastService } from '../../services/toast.service';
import { RequestResponse } from '../../models';

@Component({
  selector: 'app-approval-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatChipsModule,
    DashboardLayoutComponent,
    StatusBadgeComponent,
  ],
  templateUrl: './approval-detail.component.html',
  animations: [
    trigger('pageAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
  ],
})
export class ApprovalDetailComponent implements OnInit, OnDestroy {
  request: RequestResponse | null = null;
  commentForm: FormGroup;
  isLoading = true;
  isSubmitting = false;
  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private dataService: DataService,
    private toast: ToastService
  ) {
    this.commentForm = this.fb.group({
      comments: [''],
    });
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) this.loadRequest(id);
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
        next: (data) => { this.request = data; this.isLoading = false; },
        error: () => {
          this.isLoading = false;
          this.toast.error('Failed to load request');
          this.router.navigate(['/approvals']);
        },
      });
  }

  approve(): void {
    if (!this.request) return;
    this.isSubmitting = true;
    const comments = this.commentForm.value.comments || null;
    this.dataService.approveRequest(this.request.requestId, comments)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => { this.toast.success('Request approved'); this.router.navigate(['/approvals']); },
        error: (err) => { this.isSubmitting = false; this.toast.error(err.error?.message || 'Failed to approve'); },
      });
  }

  reject(): void {
    if (!this.request) return;
    const comments = this.commentForm.value.comments;
    if (!comments || comments.trim() === '') {
      this.toast.error('Comment is mandatory when rejecting');
      return;
    }
    this.isSubmitting = true;
    this.dataService.rejectRequest(this.request.requestId, comments)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => { this.toast.success('Request rejected'); this.router.navigate(['/approvals']); },
        error: (err) => { this.isSubmitting = false; this.toast.error(err.error?.message || 'Failed to reject'); },
      });
  }

  // "Feb 1, 2024" short format for table
  formatDate(dateStr: string | null): string {
    if (!dateStr) return '—';
    return format(new Date(dateStr), 'MMM dd, yyyy');
  }

  // "February 1st, 2024" long format for right panel
  formatDateLong(dateStr: string | null): string {
    if (!dateStr) return '—';
    return format(new Date(dateStr), 'MMMM do, yyyy');
  }

  // Gets last name initial for avatar: "John Doe" → "D"
  getLastInitial(fullName: string): string {
    if (!fullName) return '';
    const parts = fullName.trim().split(' ');
    return parts.length > 1 ? parts[parts.length - 1][0] : '';
  }

  getApprovalStatusColor(status: string): string {
    switch (status) {
      case 'Approved': return 'text-emerald-500';
      case 'Rejected': return 'text-red-500';
      default: return 'text-amber-500';
    }
  }
}