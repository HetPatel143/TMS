import { animate, style, transition, trigger } from '@angular/animations';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Subject, takeUntil } from 'rxjs';
import { DashboardLayoutComponent } from '../../../components/layout/dashboard-layout.component';
import { RequestTypeResponse, ROLES, WorkflowStepRequest } from '../../../models';
import { DataService } from '../../../services/data.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-manage-workflows',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatTooltipModule,
    DragDropModule,
    DashboardLayoutComponent,
  ],
  templateUrl: './manage-workflows.component.html',
  animations: [
    trigger('pageAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
  ],
})
export class ManageWorkflowsComponent implements OnInit, OnDestroy {
  requestTypes: RequestTypeResponse[] = [];
  selectedType: RequestTypeResponse | null = null;
  roles = ROLES.filter((r) => r.roleName !== 'Admin' && r.roleName !== 'Employee');
  workflowSteps: WorkflowStepRequest[] = [];
  isLoading = true;
  isSaving = false;
  showAddStepModal = false;
  newStepRoleId: number | null = null;
  private destroy$ = new Subject<void>();

  constructor(
    private dataService: DataService,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.loadTypes();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadTypes(): void {
    this.isLoading = true;
    this.dataService.getAllRequestTypes()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.requestTypes = data;
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
          this.toast.error('Failed to load request types');
        },
      });
  }

  selectType(type: RequestTypeResponse): void {
    this.selectedType = type;
    this.workflowSteps = type.workflowSteps.map((ws) => ({
      approvalOrder: ws.approvalOrder,
      roleId: ws.roleId,
    }));
  }

  get availableRoles() {
  const usedRoleIds = this.workflowSteps.map(s => s.roleId);
  return this.roles.filter(r => !usedRoleIds.includes(r.roleId));
}

  getRoleName(roleId: number): string {
    return ROLES.find((r) => r.roleId === roleId)?.roleName ?? 'Unknown';
  }

  getRoleColor(roleId: number): string {
    const name = this.getRoleName(roleId).toLowerCase();
    if (name.includes('manager')) return 'bg-blue-100 text-blue-700 border-blue-200';
    if (name.includes('it')) return 'bg-orange-100 text-orange-700 border-orange-200';
    if (name.includes('hr')) return 'bg-purple-100 text-purple-700 border-purple-200';
    if (name.includes('finance')) return 'bg-green-100 text-green-700 border-green-200';
    return 'bg-slate-100 text-slate-700 border-slate-200';
  }

  openAddStepModal(): void {
    this.newStepRoleId = null;
    this.showAddStepModal = true;
  }

  closeAddStepModal(): void {
    this.showAddStepModal = false;
    this.newStepRoleId = null;
  }

  addStep(): void {
    if (!this.newStepRoleId) return;
    this.workflowSteps.push({
      approvalOrder: this.workflowSteps.length + 1,
      roleId: Number(this.newStepRoleId),
    });
    this.reorderSteps();
    this.closeAddStepModal();
    this.saveWorkflow();
  }

  removeStep(index: number): void {
    this.workflowSteps.splice(index, 1);
    this.reorderSteps();
    this.saveWorkflow();
  }

  private reorderSteps(): void {
    this.workflowSteps.forEach((step, i) => (step.approvalOrder = i + 1));
  }

  moveUp(index: number): void {
  if (index === 0) return;
  [this.workflowSteps[index - 1], this.workflowSteps[index]] =
    [this.workflowSteps[index], this.workflowSteps[index - 1]];
  this.reorderSteps();
  this.saveWorkflow();
}

moveDown(index: number): void {
  if (index === this.workflowSteps.length - 1) return;
  [this.workflowSteps[index + 1], this.workflowSteps[index]] =
    [this.workflowSteps[index], this.workflowSteps[index + 1]];
  this.reorderSteps();
  this.saveWorkflow();
}

  saveWorkflow(): void {
    if (!this.selectedType) return;
    this.isSaving = true;
    this.dataService.setWorkflow(this.selectedType.requestTypeId, this.workflowSteps)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toast.success('Workflow saved successfully');
          this.isSaving = false;
          this.loadTypes();
        },
        error: (err: any) => {
          this.isSaving = false;
          this.toast.error(err.error?.message || 'Failed to save workflow');
        },
      });
  }
}