import { animate, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { format } from 'date-fns';
import { Subject, takeUntil } from 'rxjs';
import { DashboardLayoutComponent } from '../../../components/layout/dashboard-layout.component';
import { ROLES, UserResponse } from '../../../models';
import { DataService } from '../../../services/data.service';
import { ToastService } from '../../../services/toast.service';

@Component({
  selector: 'app-manage-users',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSlideToggleModule,
    MatTooltipModule,
    DashboardLayoutComponent,
  ],
  templateUrl: './manage-users.component.html',
  animations: [
    trigger('pageAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(16px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
    ]),
  ],
})
export class ManageUsersComponent implements OnInit, OnDestroy {
  displayedColumns = [ 'name', 'email', 'roleName', 'isActive', 'createdAt', 'actions'];
  users: UserResponse[] = [];
  roles = ROLES;
  isLoading = true;
  showForm = false;
  isEditing = false;
  editingUserId: number | null = null;
  userForm: FormGroup;
  searchTerm = '';
selectedRole = '';
filteredUsers: UserResponse[] = [];
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private dataService: DataService,
    private toast: ToastService
  ) {
    this.userForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      roleId: [1, Validators.required],
        isActive: [true],
    });
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadUsers(): void {
    this.isLoading = true;
    this.dataService.getAllUsers()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.users = data;
          this.filteredUsers = data;
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
          this.toast.error('Failed to load users');
        },
      });
  }

 openCreateForm(): void {
  this.showForm = true;
  this.isEditing = false;
  this.editingUserId = null;
  this.userForm.reset({ roleId: 2, isActive: true });
  this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
  this.userForm.get('password')?.updateValueAndValidity();
}
  openEditForm(user: UserResponse): void {
  this.showForm = true;
  this.isEditing = true;
  this.editingUserId = user.userId;
  this.userForm.patchValue({
    firstName: user.firstName,
    lastName: user.lastName,
    email: user.email,
    roleId: user.roleId,
    isActive: user.isActive,
    password: '',
  });
  this.userForm.get('password')?.clearValidators();
  this.userForm.get('password')?.updateValueAndValidity();
}

  closeForm(): void {
    this.showForm = false;
    this.isEditing = false;
    this.editingUserId = null;
  }

  saveUser(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    const val = this.userForm.value;

    if (this.isEditing && this.editingUserId) {const editingUser = this.users.find(u => u.userId === this.editingUserId);
const statusChanged = editingUser && editingUser.isActive !== val.isActive;

this.dataService.updateUser(this.editingUserId, {
  firstName: val.firstName,
  lastName: val.lastName,
  email: val.email,
})
  .pipe(takeUntil(this.destroy$))
  .subscribe({
    next: () => {
      const roleChange$ = this.dataService.changeUserRole(this.editingUserId!, val.roleId);
      const statusChange$ = statusChanged
        ? this.dataService.toggleUserStatus(this.editingUserId!)
        : null;

      roleChange$.pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          if (statusChange$) {
            statusChange$.pipe(takeUntil(this.destroy$)).subscribe({
              next: () => {
                this.toast.success('User saved successfully');
                this.closeForm();
                this.loadUsers();
              },
              error: () => {
                this.toast.error('User updated but status change failed');
                this.closeForm();
                this.loadUsers();
              },
            });
          } else {
            this.toast.success('User saved successfully');
            this.closeForm();
            this.loadUsers();
          }
        },
        error: () => {
          this.toast.success('User updated, but role change failed');
          this.closeForm();
          this.loadUsers();
        },
      });
    },
    error: (err) => this.toast.error(err.error?.message || 'Failed to update user'),
  });
    } else {
      this.dataService.createUser({
        firstName: val.firstName,
        lastName: val.lastName,
        email: val.email,
        password: val.password,
        roleId: val.roleId,
      })
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.toast.success('User created successfully');
            this.closeForm();
            this.loadUsers();
          },
          error: (err) => this.toast.error(err.error?.message || 'Failed to create user'),
        });
    }
  }

  toggleStatus(user: UserResponse): void {
    this.dataService.toggleUserStatus(user.userId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.toast.success('User status toggled');
          this.loadUsers();
        },
        error: () => this.toast.error('Failed to toggle status'),
      });
  }

  formatDate(dateStr: string): string {
    return format(new Date(dateStr), 'MMM dd, yyyy');
  }

  applyFilter(): void {
  const term = this.searchTerm.toLowerCase();
  this.filteredUsers = this.users.filter(u => {
    const matchesSearch = !term ||
      u.firstName.toLowerCase().includes(term) ||
      u.lastName.toLowerCase().includes(term) ||
      u.email.toLowerCase().includes(term);
    const matchesRole = !this.selectedRole || u.roleName === this.selectedRole;
    return matchesSearch && matchesRole;
  });
}

getInitials(firstName: string, lastName: string): string {
  return `${firstName?.charAt(0) ?? ''}${lastName?.charAt(0) ?? ''}`.toUpperCase();
}

getAvatarClass(role: string): string {
  const map: Record<string, string> = {
    Admin:    'bg-slate-700 text-white',
    Employee: 'bg-blue-100 text-blue-700',
    Manager:  'bg-cyan-100 text-cyan-700',
    Finance:  'bg-green-100 text-green-700',
    IT:       'bg-orange-100 text-orange-700',
    HR:       'bg-pink-100 text-pink-700',
  };
  return map[role] ?? 'bg-slate-100 text-slate-600';
}

getRoleBadgeClass(role: string): string {
  const map: Record<string, string> = {
    Admin:    'bg-slate-100 border-slate-300 text-slate-700',
    Employee: 'bg-slate-100 border-slate-200 text-slate-600',
    Manager:  'bg-cyan-50 border-cyan-200 text-cyan-700',
    Finance:  'bg-green-50 border-green-200 text-green-700',
    IT:       'bg-orange-50 border-orange-200 text-orange-700',
    HR:       'bg-pink-50 border-pink-200 text-pink-700',
  };
  return map[role] ?? 'bg-slate-100 border-slate-200 text-slate-600';
}

getRoleIcon(role: string): string {
  const map: Record<string, string> = {
    Admin:    'shield',
    Employee: 'person',
    Manager:  'group',
    Finance:  'attach_money',
    IT:       'computer',
    HR:       'favorite',
  };
  return map[role] ?? 'person';
}
}
