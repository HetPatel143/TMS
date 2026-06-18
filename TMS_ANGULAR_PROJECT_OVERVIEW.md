# TMS Angular Project - Comprehensive Overview

## Project Structure Summary

**Project Type:** Angular 17+ Standalone Components with Tailwind CSS  
**API Base:** `http://localhost:5080/api`  
**Storage:** LocalStorage (JWT tokens, user data)  

---

## 1. COMPONENT HIERARCHY & STRUCTURE

### Root Component
- **app-root** → `app.component.ts`
  - Uses RouterOutlet for page navigation
  - Standalone component configuration

### Layout Components

#### `DashboardLayoutComponent`
- **Path:** `src/app/components/layout/dashboard-layout.component.ts`
- **Purpose:** Main layout wrapper for authenticated pages
- **Features:**
  - Responsive sidebar navigation
  - Mobile-aware (uses BreakpointObserver)
  - Animated sidebar (300ms slide-in)
  - Top toolbar with user menu
  - Pending approvals badge
  - Loading progress bar
  - Dark mode toggle capability
- **Material Modules:** MatToolbar, MatSidenav, MatList, MatIcon, MatButton, MatMenu, MatBadge, MatProgressBar
- **Key Inputs:**
  - `isMobile`: Boolean flag for responsive behavior
  - `isCollapsed`: Sidebar collapse state
  - `isDarkMode`: Theme toggle

### Shared/Reusable Components

#### `StatusBadgeComponent`
- **Path:** `src/app/components/shared/status-badge.component.ts`
- **Purpose:** Display request/approval status with visual indicators
- **Input:** `status: RequestStatusType | ApprovalStatusType | string`
- **Features:**
  - Status-specific styling (Draft, Submitted, InApproval, Approved, Rejected, Cancelled, Closed)
  - Pulsing animation for "InApproval" and "Pending" states
  - Material icons per status type
  - Dynamic class mapping
- **Material Modules:** MatChips, MatIcon

#### `RoleBadgeComponent`
- **Path:** `src/app/components/shared/role-badge.component.ts`
- **Purpose:** Display user role with color-coded badges
- **Input:** `role: string`
- **Role Color Mapping:**
  - Admin → Purple (bg-purple-100 text-purple-800)
  - Employee → Blue (bg-blue-100 text-blue-800)
  - Manager → Indigo (bg-indigo-100 text-indigo-800)
  - Finance → Emerald (bg-emerald-100 text-emerald-800)
  - IT → Orange (bg-orange-100 text-orange-800)
  - HR → Pink (bg-pink-100 text-pink-800)

---

## 2. PAGE COMPONENTS & ROUTING

### Route Configuration (`app.routes.ts`)

| Path | Component | Guard | Roles | Type |
|------|-----------|-------|-------|------|
| `/` | Redirect to `/login` | - | - | Root |
| `/login` | LoginComponent | publicGuard | - | Public |
| `/dashboard` | DashboardComponent | authGuard | All | Protected |
| `/requests` | MyRequestsComponent | authGuard | Employee, Manager, Finance, IT, HR, Admin | Protected |
| `/requests/new` | CreateRequestComponent | authGuard | Employee | Protected |
| `/requests/:id` | RequestDetailComponent | authGuard | All | Protected |
| `/approvals` | PendingApprovalsComponent | authGuard | Manager, Finance, IT, HR | Protected |
| `/approvals/:id` | ApprovalDetailComponent | authGuard | Manager, Finance, IT, HR | Protected |
| `/admin/users` | ManageUsersComponent | authGuard | Admin | Admin-only |
| `/admin/request-types` | ManageRequestTypesComponent | authGuard | Admin | Admin-only |
| `/admin/workflows` | ManageWorkflowsComponent | authGuard | Admin | Admin-only |
| `/admin/requests` | AllRequestsComponent | authGuard | Admin | Admin-only |
| `**` | NotFoundComponent | - | - | Fallback |

### Page Components (12 total)

**Public Pages:**
- `pages/login/login.component.ts` - Authentication entry point

**Employee/User Pages:**
- `pages/dashboard/dashboard.component.ts` - User dashboard (role-aware)
- `pages/my-requests/my-requests.component.ts` - User's submitted requests
- `pages/create-request/create-request.component.ts` - New request form
- `pages/request-detail/request-detail.component.ts` - Request details & history

**Approver Pages:**
- `pages/pending-approvals/pending-approvals.component.ts` - List of pending approvals
- `pages/approval-detail/approval-detail.component.ts` - Single approval workflow

**Admin Pages:**
- `pages/admin/manage-users/manage-users.component.ts` - User CRUD
- `pages/admin/manage-request-types/manage-request-types.component.ts` - Request type configuration
- `pages/admin/manage-workflows/manage-workflows.component.ts` - Approval workflow setup
- `pages/admin/all-requests/all-requests.component.ts` - System-wide request view

**Error Pages:**
- `pages/not-found/not-found.component.ts` - 404 page

---

## 3. SERVICE LAYER & API INTEGRATION

### Core Services

#### `AuthService`
- **Path:** `src/app/services/auth.service.ts`
- **Purpose:** Authentication & authorization management
- **Key Methods:**
  - `login(credentials: LoginRequest): Observable<LoginResponse>` - User login
  - `logout(): void` - Clear session & navigate to login
  - `hasRole(roles: RoleName[]): boolean` - Role checking
- **Properties:**
  - `currentUser$: Observable<CurrentUser | null>` - Current user stream
  - `isLoggedIn: boolean` - Login status
  - `token: string | null` - JWT token accessor
  - `userRole: RoleName | null` - Current user role
- **Storage:** LocalStorage key: `'tms_current_user'`
- **Features:**
  - JWT token validation with expiry checking
  - Automatic localStorage persistence
  - Observable-based user state management

#### `DataService`
- **Path:** `src/app/services/data.service.ts`
- **Purpose:** Main API communication service
- **API Endpoints:**
  - **Dashboard:** `/dashboard/employee`, `/dashboard/approver`, `/dashboard/admin`
  - **Requests:** `/requests`, `/requests/{id}`, `/requests/{id}/attachments`
  - **Approvals:** `/approvals`, `/approvals/{id}/approve`, `/approvals/{id}/reject`
  - **Request Types:** `/request-types`, `/request-types/{id}`, CRUD operations
  - **Workflows:** `/workflows`, workflow step management
  - **Users:** `/users`, user management (Admin)
  - **Reports:** `/reports/summary`, `/reports/details`
- **Cached Observables:**
  - `requestTypes$: Observable<RequestTypeResponse[]>` - Request type cache
  - `users$: Observable<UserResponse[]>` - User list cache
  - `pendingApprovals$: Observable<ApprovalHistoryItem[]>` - Approval cache
  - `loading$: Observable<boolean>` - Global loading state
- **HTTP Params Support:** Pagination, filtering, date ranges

#### `ToastService`
- **Path:** `src/app/services/toast.service.ts`
- **Purpose:** User notifications (snackbars)
- **Methods:**
  - `success(message: string)` - 4s duration, green styling
  - `error(message: string)` - 6s duration, red styling
  - `info(message: string)` - 4s duration, blue styling
- **Positioning:** Top-right corner
- **Material Module:** MatSnackBar

#### `AuthInterceptor`
- **Path:** `src/app/services/auth.interceptor.ts`
- **Purpose:** HTTP interceptor for JWT token injection
- **Function:** Automatically adds `Authorization: Bearer {token}` header
- **Applied Globally:** Via `appConfig` providers

### Service Dependencies Model
```
┌─────────────────────┐
│   Components        │
└──────────┬──────────┘
           │
      ┌────┴────┐
      ▼         ▼
┌──────────┐ ┌─────────────┐
│ AuthSvc  │ │ DataService │
└────┬─────┘ └──────┬──────┘
     │               │
     │               ▼
     │         ┌─────────────┐
     │         │ HttpClient  │
     │         │  + Interceptor
     │         └──────┬──────┘
     │                │
     └────┬───────────┘
          ▼
    ToastService
    (Notifications)
```

---

## 4. STYLING & THEME CONFIGURATION

### Tailwind CSS Setup (`tailwind.config.js`)

#### Custom Color Palette

**Primary Colors (Professional Blue - HSL):**
```
primary-50: hsl(217 91% 95%)     /* Lightest */
primary-100: hsl(217 91% 90%)
primary-200: hsl(217 91% 80%)
primary-300: hsl(217 91% 70%)
primary-400: hsl(217 91% 60%)
primary-500: hsl(217 91% 50%)    /* Mid-tone */
primary-600: hsl(217 91% 45%)    /* Primary action color */
primary-700: hsl(217 91% 40%)
primary-800: hsl(217 91% 30%)
primary-900: hsl(217 91% 20%)    /* Darkest */
```

**Semantic Status Colors:**
- **Success:** hsl(142 76% 36%) | Light: hsl(142 76% 90%) | Dark: hsl(142 76% 20%)
- **Warning:** hsl(38 92% 50%) | Light: hsl(38 92% 90%) | Dark: hsl(38 92% 25%)
- **Destructive:** hsl(0 84% 60%) | Light: hsl(0 84% 90%) | Dark: hsl(0 84% 35%)
- **Info:** hsl(199 89% 48%) | Light: hsl(199 89% 90%) | Dark: hsl(199 89% 25%)

**Background & Text:**
- **background:** hsl(210 20% 98%) - Off-white
- **background-alt:** hsl(210 20% 95%) - Light gray variant
- **foreground:** hsl(222 47% 11%) - Dark text
- **foreground-muted:** hsl(222 47% 35%) - Secondary text
- **foreground-secondary:** hsl(215 14% 50%) - Tertiary text

**UI Elements:**
- **sidebar:** hsl(222 47% 11%) - Dark sidebar background
- **sidebar-light:** hsl(210 20% 90%) - Light sidebar variant
- **sidebar-text:** hsl(210 20% 96%) - Sidebar text
- **border:** hsl(214 32% 91%) - Primary border
- **border-muted:** hsl(214 32% 95%) - Light border

**Request Status Colors:**
- **status-draft:** hsl(214 32% 70%)
- **status-submitted:** hsl(217 91% 50%)
- **status-approval:** hsl(38 92% 50%) - Yellow/orange
- **status-approved:** hsl(142 76% 50%) - Green
- **status-rejected:** hsl(0 84% 60%) - Red
- **status-closed:** hsl(222 47% 40%)

#### Custom Spacing
- `sidebar: 256px` - Sidebar width constant

#### Border Radius
- `DEFAULT: 0.5rem`
- `lg: 0.75rem`
- `xl: 1rem`

#### Box Shadow Elevation System
- `DEFAULT/sm:` 0 1px 2px (subtle)
- `md:` 4px 6px (medium elevation)
- `lg:` 10px 15px (high elevation)
- `xl:` 20px 25px (maximum elevation)

#### Animations (Custom Keyframes)
- **fade-in:** 0.2s ease-in opacity transition
- **slide-down:** 0.3s ease-out from `-8px` Y
- **slide-up:** 0.3s ease-out from `+8px` Y
- **scale-in:** 0.2s ease-in scale transition
- **pulse-badge:** 2s infinite pulse (approval badges)

### Global Styles (`styles.css`)

#### Tailwind Imports
```css
@import "tailwindcss";
@import "tailwindcss/utilities";
@import "./components.css";
```

#### CSS Theme Variables
```css
--color-background: #f8fafc
--color-background-alt: #f1f5f9
--color-foreground: #0f172a
--color-foreground-secondary: #64748b
--color-border: #cbd5e1
--color-border-muted: #e2e8f0
--color-success: #10b981
--color-warning: #f59e0b
--color-destructive: #ef4444
--color-destructive-light: #fee2e2
--color-primary-{50-900}: Indigo palette
```

#### Global Reset & Typography
- **Font:** Inter, system-ui, -apple-system, sans-serif
- **Font Smoothing:** Antialiased + grayscale rendering
- **Scroll Behavior:** Smooth
- **Color Scheme:** Light

#### Heading Styles
```
h1: 1.875rem / 2.25rem (30px / 36px)
h2: 1.5rem / 2rem (24px / 32px)
h3: 1.25rem / 1.75rem (20px / 28px)
h4: 1.125rem / 1.75rem (18px / 28px)
h5: 1rem / 1.5rem (16px / 24px)
h6: 0.875rem / 1.25rem (14px / 20px)
Font-weight: 600
Color: hsl(222 47% 11%)
```

#### Scrollbar Styling
```css
Width/Height: 8px
Track: hsl(214 32% 95%)
Thumb: hsl(214 32% 91%) → hover: hsl(215 14% 50%)
Border-radius: 9999px (fully rounded)
```

#### Material Design Integration
- `.mat-mdc-card:` Custom border-radius (0.75rem) + shadow + border
- `.mat-mdc-raised-button.mat-primary:` Primary color background
- `.mat-mdc-stroked-button:` Light border styling
- `.mat-mdc-form-field-label:` (standard styling)

#### Link Styling
- **Color:** hsl(217 91% 45%) - Professional blue
- **Hover:** hsl(217 91% 40%) - Slightly darker
- **Weight:** 500 (medium)
- **Transition:** 200ms on color, background, border, fill, stroke

---

## 5. DATA MODELS & TYPE SYSTEM

### Authentication Types
```typescript
LoginRequest {
  email: string
  password: string
}

LoginResponse {
  userId: number
  fullName: string
  email: string
  roleName: RoleName
  token: string
  tokenExpiry: string
}

CurrentUser extends LoginResponse
```

### Enums & Types
```typescript
RoleName = 'Admin' | 'Employee' | 'Manager' | 'Finance' | 'IT' | 'HR'

RequestStatusType = 'Draft' | 'Submitted' | 'InApproval' | 'Approved' 
                   | 'Rejected' | 'Cancelled' | 'Closed'

ApprovalStatusType = 'Pending' | 'Approved' | 'Rejected'

FieldType = 'Text' | 'Number' | 'Date' | 'Dropdown'
```

### Request Types
```typescript
RequestResponse {
  requestId: number
  requestTypeId: number
  requestTypeName: string
  submittedBy: number
  submittedByName: string
  currentStatus: RequestStatusType
  submittedDate: string
  lastModified: string
  fieldValues: RequestFieldValueResponse[]
  attachments: RequestAttachmentResponse[]
  approvalHistory: ApprovalHistoryItem[]
}

RequestFilter {
  pageNumber: number
  pageSize: number
  status?: RequestStatusType
  requestTypeId?: number
  fromDate?: string
  toDate?: string
}

CreateRequestPayload {
  requestTypeId: number
  fieldValues: { fieldId: number, value: string }[]
  attachments?: File[]
}
```

### Dashboard Models
```typescript
EmployeeDashboard {
  totalRequests: number
  draftCount: number
  submittedCount: number
  approvedCount: number
  rejectedCount: number
  recentRequests: RequestResponse[]
}

ApproverDashboard {
  pendingApprovals: number
  approvedThisMonth: number
  rejectedThisMonth: number
  pendingApprovalsList: ApprovalHistoryItem[]
}

AdminDashboard {
  totalRequests: number
  totalUsers: number
  totalRequestTypes: number
  requestsByStatus: { status: string, count: number }[]
  systemMetrics: object
}
```

---

## 6. GUARDS & SECURITY

### Authentication Guard (`authGuard`)
- **Purpose:** Protect routes from unauthenticated access
- **Logic:**
  1. Check if user is logged in (token valid & not expired)
  2. If not logged in → redirect to `/login`
  3. If route has role requirements → verify user role
  4. If role mismatch → redirect to `/dashboard`
  5. Otherwise → allow navigation

### Public Guard (`publicGuard`)
- **Purpose:** Prevent authenticated users from accessing public pages
- **Logic:**
  1. If user is logged in → redirect to `/dashboard`
  2. Otherwise → allow access to public page (login)

---

## 7. APPLICATION CONFIGURATION

### App Config (`app.config.ts`)
```typescript
ApplicationConfig providers:
- provideZoneChangeDetection({ eventCoalescing: true })
- provideRouter(routes, withComponentInputBinding())
- provideAnimationsAsync() - Material animations
- provideHttpClient(withInterceptors([authInterceptor]))
```

### Key Features
- **Standalone Components:** All pages use standalone: true
- **Lazy Loading:** Routes use dynamic imports with loadComponent()
- **Interceptors:** AuthInterceptor automatically injected
- **Animations:** Async provider for non-blocking Material animations

---

## 8. PROJECT STATISTICS

| Category | Count |
|----------|-------|
| **Page Components** | 12 |
| **Shared Components** | 2 (StatusBadge, RoleBadge) |
| **Layout Components** | 1 (DashboardLayout) |
| **Services** | 5 (Auth, Data, Toast, AuthInterceptor, InitialData) |
| **Guards** | 2 (authGuard, publicGuard) |
| **User Roles** | 6 |
| **Request Statuses** | 7 |
| **API Endpoints** | 15+ |
| **Custom Tailwind Colors** | 25+ |
| **Custom Animations** | 5 |

---

## 9. API BASE URL & ENVIRONMENT

**Production/Development:**
- Base URL: `http://localhost:5080/api`
- Environment: Development mode

**Common Request Structure:**
```typescript
GET/POST/PUT/DELETE {API_BASE}/endpoint
Headers: {
  Authorization: Bearer {JWT_TOKEN}
  Content-Type: application/json
}

Response: {
  success: boolean
  message: string
  data: T
  errors: string[]
}
```

---

## Key Architecture Patterns

1. **Service-Based:** Centralized DataService for all API calls
2. **Observable-Driven:** RxJS Observables for async operations
3. **Lazy Loading:** Routes loaded on-demand for performance
4. **Standalone Components:** Modern Angular structure without NgModules
5. **Material Design:** Comprehensive Material library usage
6. **Role-Based Access:** Guards enforce role-based navigation
7. **Caching Strategy:** BehaviorSubjects for cached data streams
8. **Responsive Design:** BreakpointObserver for mobile adaptation
