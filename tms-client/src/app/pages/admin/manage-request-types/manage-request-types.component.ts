import { animate, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Subject, takeUntil } from 'rxjs';
import { DashboardLayoutComponent } from '../../../components/layout/dashboard-layout.component';
import { CreateFieldRequest, RequestTypeResponse } from '../../../models';
import { DataService } from '../../../services/data.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-manage-request-types',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatExpansionModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatTooltipModule,
    MatChipsModule,
    DashboardLayoutComponent,
  ],
  templateUrl: './manage-request-types.component.html',
  animations: [
    trigger('pageAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
  ],
})
export class ManageRequestTypesComponent implements OnInit, OnDestroy {
  requestTypes: RequestTypeResponse[] = [];
  isLoading = true;
  showCreateForm = false;
  isEditorOpen = false;
  editingType: RequestTypeResponse | null = null;
  createTypeForm: FormGroup;
  expandedTypeId: number | null = null;
  addFieldForm: FormGroup;
  editTypeForm: FormGroup;
  addingFieldToTypeId: number | null = null;
  editingField: any | null = null;
  editingFieldTypeId: number | null = null;
  fieldTypes = ['Text', 'Number', 'Date', 'Dropdown'];
  fieldColumns = ['fieldName', 'fieldLabel', 'fieldType', 'isRequired', 'displayOrder', 'isActive'];
  addingOptions: string[] = [];
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private dataService: DataService,
    private toast: ToastService
  ) {
    this.createTypeForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
    });
    this.addFieldForm = this.fb.group({
      fieldName: ['', Validators.required],
      fieldLabel: ['', Validators.required],
      fieldType: ['Text', Validators.required],
      isRequired: [true],
      displayOrder: [1, [Validators.required, Validators.min(1)]],
      options: [''],
    });
    this.editTypeForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      isActive: [true],
    });
  }

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

  createType(): void {
    if (this.createTypeForm.invalid) {
      this.createTypeForm.markAllAsTouched();
      return;
    }
    this.dataService.createRequestType({
      name: this.createTypeForm.value.name,
      description: this.createTypeForm.value.description || null,
      fields: [],
      workflowSteps: [],
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toast.success('Request type created');
          this.showCreateForm = false;
          this.createTypeForm.reset();
          this.loadTypes();
        },
        error: (err: any) => this.toast.error(err.error?.message || 'Failed to create'),
      });
  }

  toggleTypeStatus(type: RequestTypeResponse): void {
    this.dataService.updateRequestType(type.requestTypeId, {
      name: type.name,
      description: type.description,
      isActive: !type.isActive,
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toast.success('Status updated');
          this.loadTypes();
        },
        error: () => this.toast.error('Failed to update status'),
      });
  }

  openEditor(type: RequestTypeResponse): void {
    this.editingType = type;
    this.isEditorOpen = true;
    this.editTypeForm.patchValue({
      name: type.name,
      description: type.description || '',
      isActive: type.isActive,
    });
  }

  closeEditor(): void {
    this.isEditorOpen = false;
    this.editingType = null;
    this.addingFieldToTypeId = null;
  }

  updateType(): void {
    if (this.editTypeForm.invalid || !this.editingType) return;
    const val = this.editTypeForm.value;
    this.dataService.updateRequestType(this.editingType.requestTypeId, {
      name: val.name,
      description: val.description || null,
      isActive: val.isActive,
    }).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toast.success('Request type updated');
          this.closeEditor();
          this.loadTypes();
        },
        error: (err: any) => this.toast.error(err.error?.message || 'Failed to update'),
      });
  }

  showAddField(typeId: number): void {
    this.addingFieldToTypeId = typeId;
    this.addFieldForm.reset({ fieldType: 'Text', isRequired: true, displayOrder: 1, options: '' });
    this.addingOptions = [];
  }

  cancelAddField(): void {
    this.addingFieldToTypeId = null;
  }

  addField(): void {
    if (this.addFieldForm.invalid || !this.addingFieldToTypeId) {
      this.addFieldForm.markAllAsTouched();
      return;
    }
    const val = this.addFieldForm.value;
    const payload: CreateFieldRequest = {
      fieldName: val.fieldName,
      fieldLabel: val.fieldLabel,
      fieldType: val.fieldType,
      isRequired: val.isRequired,
      displayOrder: val.displayOrder,
      options: this.addingOptions.length ? this.addingOptions.join(',') : undefined,
    };
    this.dataService.addField(this.addingFieldToTypeId, payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toast.success('Field added');
          this.addingFieldToTypeId = null;
          this.loadTypes();
        },
        error: (err: any) => this.toast.error(err.error?.message || 'Failed to add field'),
      });
  }

  openEditField(field: any, typeId: number): void {
    this.editingField = field;
    this.editingFieldTypeId = typeId;
    this.addFieldForm.patchValue({
      fieldName: field.fieldName,
      fieldLabel: field.fieldLabel,
      fieldType: field.fieldType,
      isRequired: field.isRequired,
      displayOrder: field.displayOrder,
      options: field.options || '',
    });
    this.addingOptions = field.options ? field.options.split(',').map((o: string) => o.trim()).filter((o: string) => o) : [];
  }

  closeEditField(): void {
    this.editingField = null;
    this.editingFieldTypeId = null;
    this.addFieldForm.reset({ fieldType: 'Text', isRequired: true, displayOrder: 1 });
  }

  updateField(): void {
    if (this.addFieldForm.invalid || !this.editingField || !this.editingFieldTypeId) return;
    const val = this.addFieldForm.value;
    this.dataService.updateField(this.editingFieldTypeId, this.editingField.fieldId, {
      fieldName: val.fieldName,
      fieldLabel: val.fieldLabel,
      fieldType: val.fieldType,
      isRequired: val.isRequired,
      displayOrder: val.displayOrder,
      options: this.addingOptions.length ? this.addingOptions.join(',') : undefined,
    }).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toast.success('Field updated');
          this.closeEditField();
          this.loadTypes();
        },
        error: (err: any) => this.toast.error(err.error?.message || 'Failed to update field'),
      });
  }
  
  addOption(value: string): void {
    const v = (value || '').toString().trim();
    if (!v) return;
    if (!this.addingOptions.includes(v)) this.addingOptions.push(v);
  }

  removeOption(idx: number): void {
    this.addingOptions.splice(idx, 1);
  }
  deleteField(typeId: number, fieldId: number): void {
  this.dataService.deleteField(typeId, fieldId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: () => {
        this.toast.success('Field deleted');
        this.loadTypes();
      },
      error: (err: any) => this.toast.error(err.error?.message || 'Failed to delete field'),
    });
}

  toggleExpanded(typeId: number): void {
    this.expandedTypeId = this.expandedTypeId === typeId ? null : typeId;
  }
}