import { animate, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, map, Subject, switchMap, takeUntil, throwError } from 'rxjs';
import { DashboardLayoutComponent } from '../../components/layout/dashboard-layout.component';
import { CreateRequestPayload, FieldResponse, RequestResponse, RequestTypeResponse } from '../../models';
import { DataService } from '../../services/data.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-create-request',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatStepperModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    DashboardLayoutComponent,
  ],
  templateUrl: './create-request.component.html',
  animations: [
    trigger('pageAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
  ],
})
export class CreateRequestComponent implements OnInit, OnDestroy {
  requestTypes: RequestTypeResponse[] = [];
  selectedType: RequestTypeResponse | null = null;
  dynamicForm: FormGroup;
  isLoading = true;
  isSubmitting = false;
  isEditMode = false;
editRequestId: number | null = null;
existingRequest: RequestResponse | null = null;
  private destroy$ = new Subject<void>();

  typeIcons: Record<string, string> = {
    'Laptop Request': 'laptop',
    'Software Access Request': 'apps',
    'Reimbursement Request': 'receipt_long',
    'Leave Request': 'event_busy',
    'WFH Request': 'home_work',
  };

  constructor(
    private fb: FormBuilder,
    private dataService: DataService,
    private toast: ToastService,
    private router: Router,
    private dialog: MatDialog,
      private route: ActivatedRoute 
  ) {
    this.dynamicForm = this.fb.group({
      fields: this.fb.array([]),
    });
  }

ngOnInit(): void {
 const idParam = this.route.snapshot.paramMap.get('id');
const fullUrl = this.router.url;
this.isEditMode = fullUrl.includes('/edit');
this.editRequestId = idParam ? +idParam : null;

  this.dataService.getAllRequestTypes()
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (types) => {
        this.requestTypes = types.filter((t) => t.isActive);

        // If edit mode, load the existing request and pre-fill
        if (this.isEditMode && this.editRequestId) {
          this.dataService.getRequestById(this.editRequestId)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
              next: (req) => {
                this.existingRequest = req;
                const matchedType = this.requestTypes.find(
                  t => t.requestTypeId === req.requestTypeId
                );
                if (matchedType) {
                  this.selectedType = matchedType;
                  this.buildDynamicForm(matchedType.fields);
                  // Pre-fill field values
                  this.fieldsArray.controls.forEach(ctrl => {
                    const fieldId = ctrl.get('fieldId')?.value;
                    const existing = req.fieldValues.find(fv => fv.fieldId === fieldId);
                    if (existing) {
                      ctrl.get('value')?.setValue(existing.fieldValue);
                    }
                  });
                }
                this.isLoading = false;
              },
              error: () => {
                this.isLoading = false;
                this.toast.error('Failed to load request');
              },
            });
        } else {
          this.isLoading = false;
        }
      },
      error: () => {
        this.isLoading = false;
        this.toast.error('Failed to load request types');
      },
    });
}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get fieldsArray(): FormArray {
    return this.dynamicForm.get('fields') as FormArray;
  }

  selectType(type: RequestTypeResponse): void {
    this.selectedType = type;
    this.buildDynamicForm(type.fields);
  }

  isTypeSelected(index: number): boolean {
    if (!this.selectedType || !this.requestTypes[index]) return false;
    return this.selectedType.requestTypeId === this.requestTypes[index].requestTypeId;
  }

  private buildDynamicForm(fields: FieldResponse[]): void {
    const fieldsArr = this.fb.array(
      fields
        .filter((f) => f.isActive)
        .sort((a, b) => a.displayOrder - b.displayOrder)
        .map((field) => {
          const validators = field.isRequired ? [Validators.required] : [];
          return this.fb.group({
            fieldId: [field.fieldId],
            fieldLabel: [field.fieldLabel],
            fieldName: [field.fieldName],
            fieldType: [field.fieldType],
            isRequired: [field.isRequired],
            options: [field.options ?? ''],
            value: ['', validators],
          });
        })
    );
    this.dynamicForm = this.fb.group(
      { fields: fieldsArr },
      { validators: this.leaveDateValidator() }
    );

    // Ensure validator runs when date fields change (value may be Date or string)
    fieldsArr.controls.forEach((f) => {
      if (this.isDateField(f)) {
        const vc = f.get('value');
        if (vc) {
          vc.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(() => {
            this.dynamicForm.updateValueAndValidity({ onlySelf: true });
          });
        }
      }
    });
  }

  private leaveDateValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!this.isLeaveRequest()) return null;

      const fields = control.get('fields') as FormArray | null;
      if (!fields) return null;

      const startControl = fields.controls.find((field) => this.isLeaveStartDateField(field));
      const endControl = fields.controls.find((field) => this.isLeaveEndDateField(field));
      const startValueRaw = startControl?.get('value')?.value;
      const endValueRaw = endControl?.get('value')?.value;
      const startValue = this.toDateString(startValueRaw);
      const endValue = this.toDateString(endValueRaw);
      const errors: ValidationErrors = {};

      if (startValue && this.compareDateStrings(startValue, this.todayDateString) < 0) {
        errors['leaveStartDateInPast'] = true;
      }

      if (startValue && endValue && this.compareDateStrings(endValue, startValue) < 0) {
        errors['leaveEndDateBeforeStart'] = true;
      }

      return Object.keys(errors).length ? errors : null;
    };
  }

  private isLeaveRequest(): boolean {
    return this.normalizeText(this.selectedType?.name ?? '').includes('leave');
  }

  private isLeaveStartDateField(field: AbstractControl): boolean {
    const txt = this.fieldText(field);
    return this.isDateField(field) && (txt.includes('start') || txt.includes('from') || txt.includes('begin'));
  }

  private isLeaveEndDateField(field: AbstractControl): boolean {
    const txt = this.fieldText(field);
    return this.isDateField(field) && (txt.includes('end') || txt.includes('to') || txt.includes('until'));
  }

  private isDateField(field: AbstractControl): boolean {
    return field.get('fieldType')?.value === 'Date';
  }

  private fieldText(field: AbstractControl): string {
    return this.normalizeText(`${field.get('fieldName')?.value ?? ''} ${field.get('fieldLabel')?.value ?? ''}`);
  }

  private normalizeText(value: string): string {
    return value.toLowerCase().replace(/[^a-z0-9]+/g, '');
  }

  private compareDateStrings(left: string, right: string): number {
    const leftDate = new Date(`${left}T00:00:00`);
    const rightDate = new Date(`${right}T00:00:00`);
    return leftDate.getTime() - rightDate.getTime();
  }

  private toDateString(value: unknown): string {
    if (!value && value !== 0) return '';
    if (value instanceof Date && !isNaN(value.getTime())) {
      const y = value.getFullYear();
      const m = String(value.getMonth() + 1).padStart(2, '0');
      const d = String(value.getDate()).padStart(2, '0');
      return `${y}-${m}-${d}`;
    }
    if (typeof value === 'string') {
      if (/^\d{4}-\d{2}-\d{2}$/.test(value)) return value;
      const parsed = new Date(value);
      if (!isNaN(parsed.getTime())) {
        return this.toDateString(parsed);
      }
      return '';
    }
    return '';
  }

  get todayDateString(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  getDateMin(index: number): string | null {
    const field = this.fieldsArray.at(index);
    if (!this.isLeaveRequest() || !field) return null;
    if (this.isLeaveStartDateField(field)) return this.todayDateString;
    if (this.isLeaveEndDateField(field)) return this.getLeaveStartDateValue() || this.todayDateString;
    return null;
  }

  private getLeaveStartDateValue(): string {
    const startControl = this.fieldsArray.controls.find((field) => this.isLeaveStartDateField(field));
    const raw = startControl?.get('value')?.value;
    return this.toDateString(raw) || '';
  }

  shouldShowFieldError(index: number): boolean {
    const field = this.fieldsArray.at(index);
    const valueControl = field?.get('value');
    return !!valueControl && valueControl.invalid && (valueControl.touched || valueControl.dirty);
  }

  getFieldErrorMessage(index: number): string {
    const field = this.fieldsArray.at(index);
    const valueControl = field?.get('value');

    if (valueControl?.hasError('required')) {
      return `${field?.get('fieldLabel')?.value ?? 'This field'} is required`;
    }

    if (this.dynamicForm.hasError('leaveStartDateInPast') && field && this.isLeaveStartDateField(field)) {
      return 'Leave start date cannot be in the past';
    }

    if (this.dynamicForm.hasError('leaveEndDateBeforeStart') && field && this.isLeaveEndDateField(field)) {
      return 'Leave end date cannot be earlier than start date';
    }

    return '';
  }

  shouldShowLeaveDateError(index: number): boolean {
    const field = this.fieldsArray.at(index);
    if (!field) return false;
    const valueControl = field.get('value');
    const interacted = !!valueControl && (valueControl.touched || valueControl.dirty);

    return interacted && (
      (this.dynamicForm.hasError('leaveStartDateInPast') && this.isLeaveStartDateField(field)) ||
      (this.dynamicForm.hasError('leaveEndDateBeforeStart') && this.isLeaveEndDateField(field))
    );
  }

getFieldOptions(options: string): string[] {
  if (!options) return [];
  return options.split(',').map(o => o.trim()).filter(o => o.length > 0);
}

  getTypeIcon(name: string): string {
    return this.typeIcons[name] ?? 'description';
  }

  goBackToTypes(): void {
    this.selectedType = null;
  }

submitRequest(): void {
  if (this.dynamicForm.invalid || !this.selectedType) {
    this.dynamicForm.markAllAsTouched();
    return;
  }
  this.isSubmitting = true;
  const payload = this.buildPayload();

  if (this.isEditMode && this.editRequestId) {
    // Edit mode — call update then submit
 this.dataService.editRequest(this.editRequestId, payload)
  .pipe(
    takeUntil(this.destroy$),
    switchMap((result) =>
      this.dataService.submitRequest(result.requestId).pipe(
        map(() => result.requestId),
        catchError((err) => throwError(() => ({ submitFailedAfterSave: true, originalError: err })))
      )
    )
  )
  .subscribe({
    next: (requestId) => {
      this.toast.success('Request updated and submitted');
      this.router.navigate(['/requests', requestId]);
    },
    error: (err) => {
      this.isSubmitting = false;
      const error = err.originalError ?? err;
      this.toast.error(
        error.error?.message || (err.submitFailedAfterSave ? 'Request updated, but submission failed' : 'Failed to update request')
      );
    },
  });
  }else {
  // Create draft first, then submit in one chain
  this.dataService.createRequest(payload)
    .pipe(
      takeUntil(this.destroy$),
      switchMap((result) => 
        this.dataService.submitRequest(result.requestId).pipe(
          map(() => result.requestId),
          catchError((err) => throwError(() => ({ submitFailedAfterSave: true, originalError: err })))
        )
      )
    )
    .subscribe({
      next: (requestId) => {
        this.toast.success('Request submitted for approval');
        this.router.navigate(['/requests', requestId]);
      },
      error: (err) => {
        this.isSubmitting = false;
        const error = err.originalError ?? err;
        this.toast.error(
          error.error?.message || (err.submitFailedAfterSave ? 'Request created, but submission failed' : 'Failed to create request')
        );
      },
    });
}
}

  saveDraft(): void {
  if (!this.selectedType) return;
  this.isSubmitting = true;
  const payload = this.buildPayload();

  if (this.isEditMode && this.editRequestId) {
    this.dataService.editRequest(this.editRequestId, payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.toast.success('Draft updated successfully');
          this.router.navigate(['/requests', result.requestId]);
        },
        error: (err) => {
          this.isSubmitting = false;
          this.toast.error(err.error?.message || 'Failed to update draft');
        },
      });
  } else {
    this.dataService.saveDraft(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.toast.success('Draft saved successfully');
          this.router.navigate(['/requests', result.requestId]);
        },
        error: (err) => {
          this.isSubmitting = false;
          this.toast.error(err.error?.message || 'Failed to save draft');
        },
      });
  }
}

  private buildPayload(): CreateRequestPayload {
    const fieldsData = this.fieldsArray.value as Array<{
      fieldId: number;
      value: string;
    }>;
    return {
      requestTypeId: this.selectedType!.requestTypeId,
      fieldValues: fieldsData.map((f) => ({
        fieldId: f.fieldId,
        fieldValue: f.value?.toString() ?? '',
      })),
    };
  }

  getWorkflowRoleIcon(roleName: string): string {
  const icons: Record<string, string> = {
    'Manager': 'manage_accounts',
    'IT': 'computer',
    'Finance': 'account_balance',
    'HR': 'people',
  };
  return icons[roleName] ?? 'person';
}
}
