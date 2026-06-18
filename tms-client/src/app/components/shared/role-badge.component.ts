import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';

@Component({
  selector: 'app-role-badge',
  standalone: true,
  imports: [CommonModule, MatChipsModule],
  templateUrl: './role-badge.component.html',
})
export class RoleBadgeComponent {
  @Input() role: string = '';

  get chipColor(): string {
    const colorMap: Record<string, string> = {
      admin: 'bg-purple-100 text-purple-800',
      employee: 'bg-blue-100 text-blue-800',
      manager: 'bg-indigo-100 text-indigo-800',
      finance: 'bg-emerald-100 text-emerald-800',
      it: 'bg-orange-100 text-orange-800',
      hr: 'bg-pink-100 text-pink-800',
    };
    return colorMap[this.role.toLowerCase()] ?? 'bg-gray-100 text-gray-800';
  }
}
