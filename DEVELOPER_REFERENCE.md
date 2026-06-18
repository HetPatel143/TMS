# TMS Developer Quick Reference

## 🚀 Quick Start

### Frontend
```bash
cd tms-client
npm install
npm run dev
# Opens at http://localhost:4200
```

### Backend
```bash
cd TMS.API
dotnet restore
dotnet ef database update
dotnet run
# API at http://localhost:5000
```

### Test APIs
```bash
node test-api.js
# or use REST Client with api-test.http
```

---

## 🎨 UI/UX Reference

### Color Palette
```css
Primary Blue:     #3B7FE7 (hsl(214 91% 46%))
Success Green:    #1B7A3D (hsl(142 76% 36%))
Warning Amber:    #FFAA00 (hsl(38 92% 50%))
Danger Red:       #EE5A52 (hsl(0 84% 60%))
Info Cyan:        #1BA0DA (hsl(199 89% 48%))

Background:       #FFFFFF (pure white)
Secondary BG:     #F8FAFC (light slate)
Sidebar:          #0F172A (dark slate)

Text Primary:     #0F172A (dark slate)
Text Secondary:   #64748B (slate)
Text Muted:       #94A3B8 (light slate)

Border:           #E2E8F0 (light border)
Border Light:     #F1F5F9 (lightest border)
```

### Common Components

**Button:**
```html
<button class="btn-primary">
  <mat-icon>save</mat-icon>
  Save
</button>
```

**Card:**
```html
<div class="card-ui">
  <h3>Card Title</h3>
  <p>Card content</p>
</div>
```

**Badge:**
```html
<span class="badge-success">Approved</span>
<span class="badge-warning">Pending</span>
<span class="badge-danger">Rejected</span>
```

**Input:**
```html
<input class="input-ui" type="text" placeholder="Enter...">
```

**Stat Card:**
```html
<div class="stat-card">
  <div class="stat-label">Total Requests</div>
  <div class="stat-value">42</div>
  <mat-icon>receipt_long</mat-icon>
</div>
```

---

## 📦 Project Structure

```
TMS/
├── tms-client/                 # Angular Frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/     # Reusable UI
│   │   │   ├── pages/          # Route pages
│   │   │   ├── services/       # API services
│   │   │   ├── guards/         # Auth guards
│   │   │   └── models/         # Interfaces
│   │   ├── styles.css          # Global styles
│   │   └── components.css      # Component styles
│   └── tailwind.config.js
│
├── TMS.API/                    # .NET Backend
│   ├── Controllers/            # API endpoints
│   ├── Program.cs              # Configuration
│   └── Middleware/             # Custom middleware
│
├── TMS.Core/                   # Business Logic
│   ├── DTOs/                   # Data models
│   ├── Entities/               # DB models
│   └── Interfaces/             # Service contracts
│
└── TMS.Infrastructure/         # Data Access
    ├── Data/                   # DB context
    ├── Repositories/           # Data access
    └── Services/               # Implementation
```

---

## 🔑 Key Services

### Frontend
- **AuthService** - Authentication & tokens
- **DataService** - All API calls
- **ToastService** - Notifications

### Backend
- **IAuthService** - Auth logic
- **IRequestService** - Request operations
- **IApprovalService** - Approval workflow
- **IUserService** - User management

---

## 📊 API Endpoints Cheat Sheet

```
Auth:
  POST   /api/auth/login

Dashboard:
  GET    /api/dashboard/employee
  GET    /api/dashboard/approver
  GET    /api/dashboard/admin

Requests:
  GET    /api/requests?page=1&size=10
  GET    /api/requests/{id}
  POST   /api/requests
  PUT    /api/requests/{id}
  DELETE /api/requests/{id}
  POST   /api/requests/{id}/submit

Approvals:
  GET    /api/approvals/pending
  POST   /api/approvals/{id}/approve
  POST   /api/approvals/{id}/reject

Admin:
  GET    /api/admin/request-types
  POST   /api/admin/request-types
  GET    /api/admin/users
  POST   /api/admin/users
```

---

## 🧪 Testing Demo Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@company.com | Admin@123 |
| Employee | john.doe@company.com | Employee@123 |
| Manager | jane.smith@company.com | Manager@123 |

---

## 🐛 Common Issues & Fixes

**Port Already in Use**
```bash
# Frontend
ng serve --port 4201

# Backend
dotnet run --urls="https://localhost:5001"
```

**CORS Error**
- Check API base URL in `app.config.ts`
- Verify backend CORS is enabled

**Database Error**
```bash
dotnet ef database drop -f
dotnet ef database update
```

**Module Not Found**
```bash
npm install
npm cache clean --force
```

---

## 📝 Naming Conventions

### CSS Classes
- `card-ui` - Main component class
- `btn-primary` - Variant after dash
- `table-header-ui` - Descriptive names
- `stat-card` - Hyphenated names

### TypeScript Variables
- `camelCase` for variables
- `PascalCase` for interfaces
- `UPPER_SNAKE_CASE` for constants

### Angular Components
- `name.component.ts`
- `name.component.html`
- `name.service.ts`

---

## 🎯 Development Workflow

1. **Feature Branch**
   ```bash
   git checkout -b feature/feature-name
   ```

2. **Make Changes**
   - Update components
   - Update styles
   - Add tests

3. **Test Locally**
   ```bash
   npm run dev      # Frontend
   dotnet run       # Backend
   ```

4. **Commit**
   ```bash
   git commit -m "feat: add new feature"
   ```

5. **Push & PR**
   ```bash
   git push origin feature/feature-name
   ```

---

## 📚 Important Files

### Configuration
- `tailwind.config.js` - Theme colors
- `src/styles.css` - Global styles
- `src/components.css` - Component styles
- `app.config.ts` - Frontend config
- `appsettings.json` - Backend config

### Services
- `auth.service.ts` - Authentication
- `data.service.ts` - API calls
- `toast.service.ts` - Notifications

---

## ✅ Pre-Deployment Checklist

- [ ] All tests pass
- [ ] No console errors
- [ ] No build warnings
- [ ] All APIs respond correctly
- [ ] Responsive design verified
- [ ] Cross-browser tested
- [ ] Security reviewed
- [ ] Documentation updated
- [ ] Demo accounts work

---

## 📞 Helpful Commands

```bash
# Frontend
npm run dev         # Start dev server
npm run build       # Build for production
npm run test        # Run tests
npm run lint        # Check code quality

# Backend
dotnet run                          # Start
dotnet build                        # Compile
dotnet ef migrations add NAME       # Create migration
dotnet ef database update           # Apply migrations
dotnet publish -c Release           # Build for prod
```

---

## 🔗 Useful Links

- **Angular Docs:** https://angular.io/docs
- **Tailwind CSS:** https://tailwindcss.com/docs
- **.NET Docs:** https://learn.microsoft.com/en-us/dotnet/
- **REST API Best Practices:** https://restfulapi.net
- **Material Design:** https://material.angular.io

---

**Last Updated:** April 2026
**Version:** 1.0.0
**Maintained By:** Development Team
