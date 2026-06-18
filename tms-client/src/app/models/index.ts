/* ── Enums ──────────────────────────────────────────── */
export type RoleName = 'Admin' | 'Employee' | 'Manager' | 'Finance' | 'IT' | 'HR';

export type RequestStatusType =
  | 'Draft'
  | 'Submitted'
  | 'InApproval'
  | 'Approved'
  | 'Rejected'
  | 'Cancelled'
  | 'Closed';

export type ApprovalStatusType = 'Pending' | 'Approved' | 'Rejected';

export type FieldType = 'Text' | 'Number' | 'Date' | 'Dropdown';

/* ── API Response Wrapper ──────────────────────────── */
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

/* ── Auth ───────────────────────────────────────────── */
export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  userId: number;
  fullName: string;
  email: string;
  roleName: RoleName;
  token: string;
  tokenExpiry: string;
}

export interface CurrentUser {
  userId: number;
  fullName: string;
  email: string;
  roleName: RoleName;
  token: string;
  tokenExpiry: string;
}

/* ── User ──────────────────────────────────────────── */
export interface UserResponse {
  userId: number;
  firstName: string;
  lastName: string;
  email: string;
  roleId: number;
  roleName: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  roleId: number;
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
}

/* ── Request Type ──────────────────────────────────── */
export interface RequestTypeResponse {
  requestTypeId: number;
  name: string;
  description: string | null;
  isActive: boolean;
  createdByName: string;
  createdAt: string;
  updatedAt: string;
  fields: FieldResponse[];
  workflowSteps: WorkflowStepResponse[];
}

export interface FieldResponse {
  fieldId: number;
  fieldName: string;
  fieldLabel: string;
  fieldType: FieldType;
  isRequired: boolean;
  displayOrder: number;
  isActive: boolean;
   options?: string | null; 
}

export interface WorkflowStepResponse {
  workflowId: number;
  approvalOrder: number;
  roleId: number;
  roleName: string;
  isActive: boolean;
}

export interface CreateRequestTypeRequest {
  name: string;
  description: string | null;
  fields: CreateFieldRequest[];
  workflowSteps: WorkflowStepRequest[];
}

export interface UpdateRequestTypeRequest {
  name: string;
  description: string | null;
  isActive: boolean;
}

export interface CreateFieldRequest {
  fieldName: string;
  fieldLabel: string;
  fieldType: string;
  isRequired: boolean;
  displayOrder: number;
  options?: string | null;
}

export interface WorkflowStepRequest {
  approvalOrder: number;
  roleId: number;
}

/* ── Request ───────────────────────────────────────── */
export interface RequestResponse {
  requestId: number;
  requestNumber: string;
  requestTypeId: number;
  requestTypeName: string;
  createdByUserId: number;
  createdByName: string;
  currentStatus: RequestStatusType;
  currentApprovalOrder: number | null;
  createdAt: string;
  updatedAt: string;
  fieldValues: RequestFieldValueResponse[];
  approvals: ApprovalStepResponse[];
  statusHistory: StatusHistoryResponse[];
  attachments: AttachmentResponse[];
}

export interface RequestFieldValueResponse {
  fieldValueId: number;
  fieldId: number;
  fieldName: string;
  fieldLabel: string;
  fieldValue: string;
}

export interface ApprovalStepResponse {
  approvalId: number;
  approvalOrder: number;
  roleName: string;
  approvalStatus: ApprovalStatusType;
  approvedByName: string | null;
  comments: string | null;
  actionAt: string | null;
}

export interface StatusHistoryResponse {
  oldStatus: string;
  newStatus: string;
  changedByName: string;
  changeReason: string | null;
  changedAt: string;
}

export interface AttachmentResponse {
  attachmentId: number;
  fileName: string;
  filePath: string;
  fileType: string | null;
  uploadedByName: string;
  uploadedAt: string;
}

export interface CreateRequestPayload {
  requestTypeId: number;
  fieldValues: FieldValuePayload[];
}

export interface FieldValuePayload {
  fieldId: number;
  fieldValue: string;
}

export interface RequestFilter {
  status?: string;
  requestTypeId?: number;
  fromDate?: string;
  toDate?: string;
  pageNumber: number;
  pageSize: number;
}

/* ── Approval ──────────────────────────────────────── */
export interface ApprovalHistoryItem {
  approvalId: number;
  requestId: number;
  requestNumber: string;
  requestTypeName: string;
  requesterName: string;
  approvalOrder: number;
  roleName: string;
  approvalStatus: ApprovalStatusType;
  approvedByName: string | null;
  comments: string | null;
  actionAt: string | null;
  createdAt: string;
}

export interface ApprovalAction {
  action: string;
  comments: string | null;
}

/* ── Dashboard ─────────────────────────────────────── */
export interface EmployeeDashboard {
  totalRequests: number;
  draftCount: number;
  submittedCount: number;
  inApprovalCount: number;
  approvedCount: number;
  rejectedCount: number;
  cancelledCount: number;
  closedCount: number;
  recentRequests: RecentRequest[];
}

export interface RecentRequest {
  requestId: number;
  requestNumber: string;
  requestTypeName: string;
  currentStatus: RequestStatusType;
  createdAt: string;
  updatedAt: string;
}

export interface ApproverDashboard {
  totalPendingApprovals: number;
  approvedCount: number;
  rejectedCount: number;
   totalMyRequests: number;
  myApprovedCount: number;
  pendingApprovals: PendingApprovalItem[];
}

export interface PendingApprovalItem {
  requestId: number;
  requestNumber: string;
  requestTypeName: string;
  requesterName: string;
  approvalOrder: number;
  submittedAt: string;
}

export interface AdminDashboard {
  totalUsers: number;
  totalRequestTypes: number;
  totalRequests: number;
  pendingApprovalsCount: number;
  requestsByStatus: Record<string, number>;
  requestTypeStats: RequestTypeStats[];
   recentActivity: RecentActivity[]; 
}

export interface RequestTypeStats {
  requestTypeName: string;
  description: string | null;  
  totalRequests: number;
  // avgApprovalDurationHours: number;
}
export interface RecentActivity {   // ← add this new interface
  requestId: number;
  requestNumber: string;
  requestTypeName: string;
  requesterName: string;
  currentStatus: RequestStatusType;
  createdAt: string;
}


/* ── Report ────────────────────────────────────────── */
export interface ReportFilter {
  requestTypeId?: number;
  status?: string;
  approvalRoleId?: number;
  fromDate?: string;
  toDate?: string;
}

/* ── Role mapping ──────────────────────────────────── */
export interface RoleInfo {
  roleId: number;
  roleName: RoleName;
}

export const ROLES: RoleInfo[] = [
  { roleId: 1, roleName: 'Employee' },
  { roleId: 2, roleName: 'Manager' },
  { roleId: 3, roleName: 'Finance' },
  { roleId: 4, roleName: 'IT' },
  { roleId: 5, roleName: 'HR' },
  { roleId: 6, roleName: 'Admin' },
];

/* ── Demo accounts for login page ──────────────────── */
export interface DemoAccount {
  email: string;
  password: string;
  name: string;
  role: RoleName;
  avatar: string;
}

export const DEMO_ACCOUNTS: DemoAccount[] = [
  { email: 'admin@tms.com', password: 'Admin@123', name: 'System Admin', role: 'Admin', avatar: 'SA' },
  { email: 'vikram.singh@tms.com', password: 'Password@123', name: 'Vikram Singh', role: 'Manager', avatar: 'VS' },
  { email: 'rajesh.kumar@tms.com', password: 'Password@123', name: 'Rajesh Kumar', role: 'IT', avatar: 'RK' },
  { email: 'anita.desai@tms.com', password: 'Password@123', name: 'Anita Desai', role: 'Finance', avatar: 'AD' },
  { email: 'sneha.gupta@tms.com', password: 'Password@123', name: 'Sneha Gupta', role: 'HR', avatar: 'SG' },
  { email: 'rahul.sharma@tms.com', password: 'Password@123', name: 'Rahul Sharma', role: 'Employee', avatar: 'RS' },
  { email: 'priya.patel@tms.com', password: 'Password@123', name: 'Priya Patel', role: 'Employee', avatar: 'PP' },
];

/* ── Nav item ──────────────────────────────────────── */
export interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles: RoleName[];
  badge?: number;
}
