import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { RequestStatusType, ApprovalStatusType } from '../../models';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule, MatChipsModule, MatIconModule],
  templateUrl: './status-badge.component.html',
})
export class StatusBadgeComponent {
  @Input() status: RequestStatusType | ApprovalStatusType | string = '';

  get chipClass(): string {
    const statusLower = this.status.toLowerCase().replace(/\s+/g, '');
    const classMap: Record<string, string> = {
      draft: 'chip-draft',
      submitted: 'chip-submitted',
      inapproval: 'chip-inapproval',
      approved: 'chip-approved',
      rejected: 'chip-rejected',
      cancelled: 'chip-cancelled',
      closed: 'chip-closed',
      pending: 'chip-pending',
    };
    return classMap[statusLower] ?? 'chip-draft';
  }

  get shouldPulse(): boolean {
    return this.status.toLowerCase().replace(/\s+/g, '') === 'inapproval' ||
           this.status.toLowerCase() === 'pending';
  }

  get icon(): string {
  const statusLower = this.status.toLowerCase().replace(/\s+/g, '');
  const iconMap: Record<string, string> = {
    draft: 'draft',
    submitted: 'upload',
    inapproval: 'schedule',
    approved: 'check_circle',
    rejected: 'cancel',
    cancelled: 'block',
    closed: 'lock',
    pending: 'schedule',
  };
  return iconMap[statusLower] ?? 'info';
}
}
