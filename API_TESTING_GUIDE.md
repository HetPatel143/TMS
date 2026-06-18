# TMS API Testing Guide

## Base URL
- Development: `http://localhost:5000/api`

## Demo Accounts
```
Admin:
- Email: admin@company.com
- Password: Admin@123

Employee:
- Email: john.doe@company.com
- Password: Employee@123

Manager:
- Email: jane.smith@company.com
- Password: Manager@123

Finance:
- Email: mike.johnson@company.com
- Password: Finance@123

IT:
- Email: sarah.wilson@company.com
- Password: IT@123

HR:
- Email: emily.brown@company.com
- Password: HR@123
```

## API Endpoints Checklist

### Authentication
- [x] POST `/auth/login` - User login

### Dashboard
- [x] GET `/dashboard/employee` - Employee dashboard data
- [x] GET `/dashboard/approver` - Approver dashboard data
- [x] GET `/dashboard/admin` - Admin dashboard data

### Requests
- [x] GET `/requests` - Get all requests (with pagination & filtering)
- [x] GET `/requests/{id}` - Get single request
- [x] POST `/requests` - Create request
- [x] POST `/requests/draft` - Save as draft
- [x] PUT `/requests/{id}` - Update request
- [x] POST `/requests/{id}/submit` - Submit request
- [x] DELETE `/requests/{id}` - Delete request

### Approvals
- [x] GET `/approvals/pending` - Get pending approvals
- [x] GET `/approvals/{requestId}/history` - Get approval history
- [x] POST `/approvals/{requestId}/approve` - Approve request
- [x] POST `/approvals/{requestId}/reject` - Reject request

### Request Types (Admin)
- [x] GET `/admin/request-types` - Get all request types
- [x] GET `/admin/request-types/{id}` - Get single request type
- [x] POST `/admin/request-types` - Create request type
- [x] PUT `/admin/request-types/{id}` - Update request type

### Fields (Admin)
- [x] POST `/admin/request-types/{requestTypeId}/fields` - Add field
- [x] PUT `/admin/request-types/{requestTypeId}/fields/{fieldId}` - Update field
- [x] DELETE `/admin/request-types/{requestTypeId}/fields/{fieldId}` - Delete field

### Workflows (Admin)
- [x] GET `/admin/request-types/{requestTypeId}/workflows` - Get workflows
- [x] POST `/admin/request-types/{requestTypeId}/workflows` - Set workflow steps

### Users (Admin)
- [x] GET `/admin/users` - Get all users
- [x] GET `/admin/users/{id}` - Get single user
- [x] POST `/admin/users` - Create user
- [x] PUT `/admin/users/{id}` - Update user
- [x] PUT `/admin/users/{id}/role` - Update user role
- [x] PUT `/admin/users/{id}/status` - Deactivate/activate user

### Reports
- [x] GET `/reports` - Get filtered reports
- [x] GET `/reports/export` - Export reports as CSV/Excel

## Testing Steps

### 1. Frontend Build & Run
```bash
cd tms-client
npm install
npm run dev
```
Visit: http://localhost:4200

### 2. Backend Build & Run
```bash
cd TMS.API
dotnet build
dotnet run
```
API runs on: http://localhost:5000

### 3. API Testing with REST Client
- Use the provided `api-test.http` file in VS Code with REST Client extension
- Update the `@token` variable after login
- Execute requests to test each endpoint

## Key Validation Points

✓ All authentication endpoints work
✓ CORS is properly configured
✓ JWT token validation works
✓ Role-based access control enforced
✓ Request validation works
✓ Database migrations execute
✓ Error handling works (400, 401, 403, 404, 500)

## Common Issues & Solutions

### Issue: CORS errors
- Ensure API CORS policy allows frontend origin
- Check `Program.cs` for CORS configuration

### Issue: 401 Unauthorized
- Ensure JWT token is included in Authorization header
- Token format: `Bearer <token>`

### Issue: Database connection fails
- Check connection string in `appsettings.json`
- Ensure database exists and migrations ran

### Issue: File uploads fail
- Check `wwwroot/uploads` directory exists
- Verify file upload size limits

## Performance Tips
- Requests are paginated (default 10 items)
- Use filters to reduce data size
- Implement request caching on frontend
- Monitor database query performance
