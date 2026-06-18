import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, finalize, map, Observable, tap } from 'rxjs';
import {
  AdminDashboard,
  ApiResponse,
  ApprovalHistoryItem,
  ApproverDashboard,
  CreateFieldRequest,
  CreateRequestPayload,
  CreateRequestTypeRequest,
  CreateUserRequest,
  EmployeeDashboard,
  FieldResponse,
  PagedResult,
  ReportFilter,
  RequestFilter,
  RequestResponse,
  RequestTypeResponse,
  UpdateRequestTypeRequest,
  UpdateUserRequest,
  UserResponse,
  WorkflowStepRequest,
  WorkflowStepResponse,
} from '../models';

const API_BASE = 'http://localhost:5080/api';

@Injectable({ providedIn: 'root' })
export class DataService {
  /* ── Loading State ─────────────────────────────────── */
  private loadingSubject = new BehaviorSubject<boolean>(false);
  readonly loading$ = this.loadingSubject.asObservable();

  /* ── Request Types Cache ───────────────────────────── */
  private requestTypesSubject = new BehaviorSubject<RequestTypeResponse[]>([]);
  private requestTypesLoaded = false;
  readonly requestTypes$ = this.requestTypesSubject.asObservable();

  /* ── Users Cache ───────────────────────────────────── */
  private usersSubject = new BehaviorSubject<UserResponse[]>([]);
  readonly users$ = this.usersSubject.asObservable();

  /* ── Pending Approvals Cache ───────────────────────── */
  private pendingApprovalsSubject = new BehaviorSubject<ApprovalHistoryItem[]>([]);
  readonly pendingApprovals$ = this.pendingApprovalsSubject.asObservable();

  constructor(private http: HttpClient) {}

  private setLoading(loading: boolean): void {
    this.loadingSubject.next(loading);
  }

  private invalidateRequestTypesCache(): void {
    this.requestTypesLoaded = false;
  }

  /* ══════════════════════════════════════════════════════
     DASHBOARD
     ══════════════════════════════════════════════════════ */
  getEmployeeDashboard(): Observable<EmployeeDashboard> {
    this.setLoading(true);
    return this.http.get<ApiResponse<EmployeeDashboard>>(`${API_BASE}/dashboard/employee`).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  getApproverDashboard(): Observable<ApproverDashboard> {
    this.setLoading(true);
    return this.http.get<ApiResponse<ApproverDashboard>>(`${API_BASE}/dashboard/approver`).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  getAdminDashboard(): Observable<AdminDashboard> {
    this.setLoading(true);
    return this.http.get<ApiResponse<AdminDashboard>>(`${API_BASE}/dashboard/admin`).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  /* ══════════════════════════════════════════════════════
     REQUESTS
     ══════════════════════════════════════════════════════ */
  getMyRequests(filter: RequestFilter): Observable<PagedResult<RequestResponse>> {
    this.setLoading(true);
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber.toString())
      .set('pageSize', filter.pageSize.toString());
    if (filter.status) params = params.set('status', filter.status);
    if (filter.requestTypeId) params = params.set('requestTypeId', filter.requestTypeId.toString());
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);

    return this.http.get<ApiResponse<PagedResult<RequestResponse>>>(`${API_BASE}/requests`, { params }).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  getRequestById(id: number): Observable<RequestResponse> {
    this.setLoading(true);
    return this.http.get<ApiResponse<RequestResponse>>(`${API_BASE}/requests/${id}`).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  createRequest(payload: CreateRequestPayload): Observable<RequestResponse> {
    this.setLoading(true);
    return this.http.post<ApiResponse<RequestResponse>>(`${API_BASE}/requests`, payload).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  saveDraft(payload: CreateRequestPayload): Observable<RequestResponse> {
    this.setLoading(true);
    return this.http.post<ApiResponse<RequestResponse>>(`${API_BASE}/requests/draft`, payload).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  editRequest(id: number, payload: CreateRequestPayload): Observable<RequestResponse> {
    this.setLoading(true);
    return this.http.put<ApiResponse<RequestResponse>>(`${API_BASE}/requests/${id}`, payload).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  submitRequest(id: number): Observable<RequestResponse> {
    this.setLoading(true);
    return this.http.post<ApiResponse<RequestResponse>>(`${API_BASE}/requests/${id}/submit`, {}).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  cancelRequest(id: number): Observable<void> {
    this.setLoading(true);
    return this.http.delete<ApiResponse<void>>(`${API_BASE}/requests/${id}`).pipe(
      map(() => undefined),
      finalize(() => this.setLoading(false))
    );
  }

  /* ══════════════════════════════════════════════════════
     APPROVALS
     ══════════════════════════════════════════════════════ */
  getPendingApprovals(): Observable<ApprovalHistoryItem[]> {
    this.setLoading(true);
    return this.http.get<ApiResponse<ApprovalHistoryItem[]>>(`${API_BASE}/approvals/pending`).pipe(
      map((res) => res.data),
      tap((data) => this.pendingApprovalsSubject.next(data)),
      finalize(() => this.setLoading(false))
    );
  }

  approveRequest(requestId: number, comments: string | null): Observable<void> {
  this.setLoading(true);
  return this.http.post<ApiResponse<void>>(`${API_BASE}/approvals/${requestId}/approve`, {
    action: 'Approve',
    comments: comments ?? null,
  }).pipe(
    map(() => undefined),
    finalize(() => this.setLoading(false))
  );
}

rejectRequest(requestId: number, comments: string): Observable<void> {
  this.setLoading(true);
  return this.http.post<ApiResponse<void>>(`${API_BASE}/approvals/${requestId}/reject`, {
    action: 'Reject',
    comments: comments,
  }).pipe(
    map(() => undefined),
    finalize(() => this.setLoading(false))
  );
}

  getApprovalHistory(requestId: number): Observable<ApprovalHistoryItem[]> {
    return this.http.get<ApiResponse<ApprovalHistoryItem[]>>(`${API_BASE}/approvals/${requestId}/history`).pipe(
      map((res) => res.data)
    );
  }

  /* ══════════════════════════════════════════════════════
     REQUEST TYPES (Admin)
     ══════════════════════════════════════════════════════ */
  getAllRequestTypes(): Observable<RequestTypeResponse[]> {
      if (this.requestTypesLoaded) {
    return this.requestTypes$;
  }
    this.setLoading(true);
    return this.http.get<ApiResponse<RequestTypeResponse[]>>(`${API_BASE}/admin/request-types`).pipe(
      map((res) => res.data),
      tap((data) => {
        this.requestTypesSubject.next(data);
        this.requestTypesLoaded = true;
      }),
      finalize(() => this.setLoading(false))
    );
  }

  getRequestTypeById(id: number): Observable<RequestTypeResponse> {
    return this.http.get<ApiResponse<RequestTypeResponse>>(`${API_BASE}/admin/request-types/${id}`).pipe(
      map((res) => res.data)
    );
  }

  createRequestType(payload: CreateRequestTypeRequest): Observable<RequestTypeResponse> {
    this.setLoading(true);
    return this.http.post<ApiResponse<RequestTypeResponse>>(`${API_BASE}/admin/request-types`, payload).pipe(
      map((res) => res.data),
      tap(() => this.invalidateRequestTypesCache()),
      finalize(() => this.setLoading(false))
    );
  }

  updateRequestType(id: number, payload: UpdateRequestTypeRequest): Observable<RequestTypeResponse> {
    this.setLoading(true);
    return this.http.put<ApiResponse<RequestTypeResponse>>(`${API_BASE}/admin/request-types/${id}`, payload).pipe(
      map((res) => res.data),
      tap(() => this.invalidateRequestTypesCache()),
      finalize(() => this.setLoading(false))
    );
  }

  addField(requestTypeId: number, payload: CreateFieldRequest): Observable<FieldResponse> {
    this.setLoading(true);
    return this.http.post<ApiResponse<FieldResponse>>(`${API_BASE}/admin/request-types/${requestTypeId}/fields`, payload).pipe(
      map((res) => res.data),
      tap(() => this.invalidateRequestTypesCache()),
      finalize(() => this.setLoading(false))
    );
  }

  updateField(requestTypeId: number, fieldId: number, payload: CreateFieldRequest): Observable<FieldResponse> {
    this.setLoading(true);
    return this.http.put<ApiResponse<FieldResponse>>(`${API_BASE}/admin/request-types/${requestTypeId}/fields/${fieldId}`, payload).pipe(
      map((res) => res.data),
      tap(() => this.invalidateRequestTypesCache()),
      finalize(() => this.setLoading(false))
    );
  }

  deleteField(requestTypeId: number, fieldId: number): Observable<void> {
  this.setLoading(true);
  return this.http.delete<ApiResponse<void>>(
    `${API_BASE}/admin/request-types/${requestTypeId}/fields/${fieldId}`
  ).pipe(
    map(() => undefined),
    tap(() => this.invalidateRequestTypesCache()),
    finalize(() => this.setLoading(false))
  );
}

  setWorkflow(requestTypeId: number, steps: WorkflowStepRequest[]): Observable<void> {
    this.setLoading(true);
    return this.http.post<ApiResponse<void>>(`${API_BASE}/admin/request-types/${requestTypeId}/workflows`, steps).pipe(
      map(() => undefined),
      tap(() => this.invalidateRequestTypesCache()),
      finalize(() => this.setLoading(false))
    );
  }

  getWorkflow(requestTypeId: number): Observable<WorkflowStepResponse[]> {
    return this.http.get<ApiResponse<WorkflowStepResponse[]>>(`${API_BASE}/admin/request-types/${requestTypeId}/workflows`).pipe(
      map((res) => res.data)
    );
  }

  /* ══════════════════════════════════════════════════════
     USERS (Admin)
     ══════════════════════════════════════════════════════ */
  getAllUsers(): Observable<UserResponse[]> {
    this.setLoading(true);
    return this.http.get<ApiResponse<UserResponse[]>>(`${API_BASE}/admin/users`).pipe(
      map((res) => res.data),
      tap((data) => this.usersSubject.next(data)),
      finalize(() => this.setLoading(false))
    );
  }

  getUserById(id: number): Observable<UserResponse> {
    return this.http.get<ApiResponse<UserResponse>>(`${API_BASE}/admin/users/${id}`).pipe(
      map((res) => res.data)
    );
  }

  createUser(payload: CreateUserRequest): Observable<UserResponse> {
    this.setLoading(true);
    return this.http.post<ApiResponse<UserResponse>>(`${API_BASE}/admin/users`, payload).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  updateUser(id: number, payload: UpdateUserRequest): Observable<UserResponse> {
    this.setLoading(true);
    return this.http.put<ApiResponse<UserResponse>>(`${API_BASE}/admin/users/${id}`, payload).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  changeUserRole(userId: number, roleId: number): Observable<void> {
    this.setLoading(true);
    return this.http.put<ApiResponse<void>>(`${API_BASE}/admin/users/${userId}/role`, { roleId }).pipe(
      map(() => undefined),
      finalize(() => this.setLoading(false))
    );
  }

  toggleUserStatus(userId: number): Observable<void> {
    this.setLoading(true);
    return this.http.put<ApiResponse<void>>(`${API_BASE}/admin/users/${userId}/status`, {}).pipe(
      map(() => undefined),
      finalize(() => this.setLoading(false))
    );
  }

  /* ══════════════════════════════════════════════════════
     REPORTS (Admin)
     ══════════════════════════════════════════════════════ */
  getReports(filter: ReportFilter): Observable<RequestResponse[]> {
    this.setLoading(true);
    let params = new HttpParams();
    if (filter.requestTypeId) params = params.set('requestTypeId', filter.requestTypeId.toString());
    if (filter.status) params = params.set('status', filter.status);
    if (filter.approvalRoleId) params = params.set('approvalRoleId', filter.approvalRoleId.toString());
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);

    return this.http.get<ApiResponse<RequestResponse[]>>(`${API_BASE}/reports`, { params }).pipe(
      map((res) => res.data),
      finalize(() => this.setLoading(false))
    );
  }

  exportReportsCsv(filter: ReportFilter): Observable<Blob> {
    let params = new HttpParams().set('format', 'csv');
    if (filter.requestTypeId) params = params.set('requestTypeId', filter.requestTypeId.toString());
    if (filter.status) params = params.set('status', filter.status);
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);

    return this.http.get(`${API_BASE}/reports/export`, { params, responseType: 'blob' });
  }
}
