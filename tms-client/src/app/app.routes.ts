import { Routes } from '@angular/router';
import { authGuard, publicGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  {
    path: 'login',
    canActivate: [publicGuard],
    loadComponent: () => import('./pages/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/dashboard/dashboard.component').then((m) => m.DashboardComponent),
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/profile/profile.component').then((m) => m.ProfileComponent),
  },
  {
    path: 'requests',
    canActivate: [authGuard],
    data: { roles: ['Employee', 'Manager', 'Finance', 'IT', 'HR', 'Admin'] },
    loadComponent: () => import('./pages/my-requests/my-requests.component').then((m) => m.MyRequestsComponent),
  },
  {
    path: 'requests/new',
    canActivate: [authGuard],
    data: { roles: ['Employee', 'Manager', 'Finance', 'IT', 'HR'] },
    loadComponent: () => import('./pages/create-request/create-request.component').then((m) => m.CreateRequestComponent),
  },
    {
  path: 'requests/:id/edit',
  canActivate: [authGuard],
  data: { roles: ['Employee', 'Manager', 'Finance', 'IT', 'HR'] },
  loadComponent: () => import('./pages/create-request/create-request.component').then((m) => m.CreateRequestComponent),
},
  {
    path: 'requests/:id',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/request-detail/request-detail.component').then((m) => m.RequestDetailComponent),
  },

  {
    path: 'approvals',
    canActivate: [authGuard],
    data: { roles: ['Manager', 'Finance', 'IT', 'HR'] },
    loadComponent: () => import('./pages/pending-approvals/pending-approvals.component').then((m) => m.PendingApprovalsComponent),
  },
  {
    path: 'approvals/:id',
    canActivate: [authGuard],
    data: { roles: ['Manager', 'Finance', 'IT', 'HR'] },
    loadComponent: () => import('./pages/approval-detail/approval-detail.component').then((m) => m.ApprovalDetailComponent),
  },
  {
    path: 'admin/users',
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
    loadComponent: () => import('./pages/admin/manage-users/manage-users.component').then((m) => m.ManageUsersComponent),
  },
  {
    path: 'admin/request-types',
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
    loadComponent: () => import('./pages/admin/manage-request-types/manage-request-types.component').then((m) => m.ManageRequestTypesComponent),
  },
  {
    path: 'admin/workflows',
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
    loadComponent: () => import('./pages/admin/manage-workflows/manage-workflows.component').then((m) => m.ManageWorkflowsComponent),
  },
  {
    path: 'admin/requests',
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
    loadComponent: () => import('./pages/admin/all-requests/all-requests.component').then((m) => m.AllRequestsComponent),
  },
  {
    path: '**',
    loadComponent: () => import('./pages/not-found/not-found.component').then((m) => m.NotFoundComponent),
  },
];
