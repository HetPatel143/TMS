# TMS - Ticket Management System
## Complete Setup and Deployment Guide

---

## 📋 Prerequisites

### System Requirements
- Node.js 18+ (for Angular frontend)
- .NET 8+ (for C# backend)
- SQL Server 2019+ (or SQL Server Express)
- Visual Studio Code or Visual Studio 2022

### Tools Required
```bash
# Node Package Manager
npm --version  # Should be 9+

# .NET CLI
dotnet --version  # Should be 8.0+

# Git
git --version
```

---

## 🚀 Frontend Setup (Angular)

### 1. Install Dependencies
```bash
cd tms-client
npm install
```

### 2. Configuration
Check `src/app/app.config.ts` for API base URL:
```typescript
export const API_BASE = 'http://localhost:5000/api';
```

### 3. Development Server
```bash
npm run dev
# Opens on http://localhost:4200
```

### 4. Production Build
```bash
npm run build
# Output in dist/ folder
```

### 5. Running Tests
```bash
npm run test
```

---

## 🔧 Backend Setup (.NET)

### 1. Install Dependencies
```bash
cd TMS.API
dotnet restore
```

### 2. Database Configuration
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TMS;Trusted_Connection=true;"
  }
}
```

### 3. Database Migration
```bash
dotnet ef database update
# Creates database and runs all migrations
```

### 4. Development Server
```bash
dotnet run
# API runs on http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### 5. Production Build
```bash
dotnet publish -c Release -o ./publish
```

---

## ✅ API Endpoint Verification

### Quick Test - Use VS Code REST Client
1. Install "REST Client" extension
2. Open `api-test.http`
3. Update `@baseUrl` and `@token` as needed
4. Execute requests by clicking "Send Request"

### Automated Testing
```bash
node test-api.js
# Runs comprehensive API test suite
```

---

## 📊 Demo Accounts

Use these credentials to test the application:

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@company.com | Admin@123 |
| Employee | john.doe@company.com | Employee@123 |
| Manager | jane.smith@company.com | Manager@123 |
| Finance | mike.johnson@company.com | Finance@123 |
| IT | sarah.wilson@company.com | IT@123 |
| HR | emily.brown@company.com | HR@123 |

---

## 📁 Project Structure

### Frontend (`tms-client/`)
```
src/
├── app/
│   ├── components/          # Reusable UI components
│   ├── guards/              # Route guards
│   ├── models/              # TypeScript interfaces
│   ├── pages/               # Page components
│   ├── services/            # API services
│   ├── app.config.ts        # Configuration
│   ├── app.routes.ts        # Route definitions
│   └── app.component.ts     # Root component
├── styles.css               # Global styles
├── components.css           # Component styles
└── main.ts                  # Entry point
```

### Backend (`TMS.API/`)
```
Controllers/                 # API endpoints
├── AuthController.cs
├── DashboardController.cs
├── RequestController.cs
├── ApprovalController.cs
├── AdminUserController.cs
├── AdminRequestTypeController.cs
└── ReportController.cs

Models/                      # DTO models
Middleware/                  # Custom middleware
Extensions/                  # Extension methods
Properties/                  # Launch settings
Program.cs                   # Startup configuration
```

### Core Library (`TMS.Core/`)
```
DTOs/                        # Data Transfer Objects
Entities/                    # Database entities
Enums/                       # Enumerations
Exceptions/                  # Custom exceptions
Interfaces/                  # Service interfaces
```

### Infrastructure (`TMS.Infrastructure/`)
```
Data/                        # Database context
├── TMSDbContext.cs
├── Configurations/
└── Migrations/
Helpers/                     # Utility helpers
Repositories/                # Data access
Services/                    # Business logic
Mapping/                     # AutoMapper profiles
```

---

## 🔐 Security Features

### Authentication
- JWT token-based authentication
- Secure password hashing (BCrypt)
- Token refresh mechanism
- Account lockout on failed attempts

### Authorization
- Role-based access control (RBAC)
- Route guards on frontend
- Endpoint authorization on backend

### Data Protection
- SQL injection prevention (parameterized queries)
- XSS protection
- CORS configuration
- Input validation

---

## 📝 API Endpoints Summary

### Auth
- `POST /api/auth/login` - User authentication

### Dashboard
- `GET /api/dashboard/employee` - Employee dashboard
- `GET /api/dashboard/approver` - Approver dashboard
- `GET /api/dashboard/admin` - Admin dashboard

### Requests
- `GET /api/requests` - List all requests
- `GET /api/requests/{id}` - Get request details
- `POST /api/requests` - Create request
- `PUT /api/requests/{id}` - Update request
- `DELETE /api/requests/{id}` - Delete request

### Approvals
- `GET /api/approvals/pending` - Get pending approvals
- `POST /api/approvals/{id}/approve` - Approve request
- `POST /api/approvals/{id}/reject` - Reject request

### Admin - Request Types
- `GET /api/admin/request-types` - List request types
- `POST /api/admin/request-types` - Create request type
- `PUT /api/admin/request-types/{id}` - Update request type

### Admin - Users
- `GET /api/admin/users` - List users
- `POST /api/admin/users` - Create user
- `PUT /api/admin/users/{id}` - Update user

### Reports
- `GET /api/reports` - Generate reports
- `GET /api/reports/export` - Export as CSV/Excel

---

## 🐛 Troubleshooting

### Frontend Issues

**Port 4200 already in use:**
```bash
ng serve --port 4201
```

**CORS errors:**
- Verify backend CORS is enabled in `Program.cs`
- Check frontend API_BASE URL matches backend

**Module not found:**
```bash
npm install
npm cache clean --force
rm -rf node_modules
```

### Backend Issues

**Database connection failed:**
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database exists

**Port 5000 already in use:**
```bash
dotnet run --urls="https://localhost:5001"
```

**Migrations failed:**
```bash
dotnet ef database drop -f
dotnet ef database update
```

---

## 📈 Performance Tips

### Frontend
- Enable lazy loading for route modules
- Use `OnPush` change detection
- Implement virtual scrolling for large lists
- Cache API responses appropriately

### Backend
- Use pagination for list endpoints
- Implement database indexing
- Enable caching headers
- Use async/await throughout
- Monitor query performance

---

## 🚀 Deployment

### Docker Deployment

**Frontend:**
```dockerfile
FROM node:18-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
```

**Backend:**
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "TMS.API.dll"]
```

### Environment Variables
```bash
# Backend
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=...
JWT_SECRET=...

# Frontend
API_BASE_URL=https://api.example.com/api
```

---

## 📞 Support

For issues or questions:
1. Check the logs first
2. Review API_TESTING_GUIDE.md
3. Check troubleshooting section
4. Review source code comments

---

## 📄 License

Internal Use Only - TMS Enterprise System

---

**Last Updated:** April 2026
**Version:** 1.0.0
