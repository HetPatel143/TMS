# TMS Client — Angular Frontend

A modern Angular 19 frontend for the Ticket Management System (TMS) that connects to the .NET Core 8 backend API.

## Tech Stack

- **Angular 19** — standalone components, no NgModules
- **Tailwind CSS v4** — utility-first styling
- **Angular Material v19** — UI component library
- **RxJS** — BehaviorSubject state management
- **date-fns** — date formatting
- **TypeScript** — strict mode

## Getting Started

```bash
# Install dependencies
npm install

# Start dev server (API must be running on localhost:5080)
ng serve

# Open http://localhost:4200
```

## Demo Accounts

| Email                     | Password      | Role     |
|---------------------------|---------------|----------|
| admin@company.com         | admin123      | Admin    |
| john.doe@company.com      | password123   | Employee |
| jane.smith@company.com    | password123   | Manager  |
| mike.johnson@company.com  | password123   | Finance  |
| sarah.wilson@company.com  | password123   | IT       |
| emily.brown@company.com   | password123   | HR       |
| david.lee@company.com     | password123   | Employee |

## Project Structure

```
src/app/
├── models/          → TypeScript interfaces
├── services/        → API services, auth, toast
├── guards/          → Route guards
├── components/      → Layout & shared components
└── pages/           → Feature page components
```

## Backend API

The API runs on `http://localhost:5080`. Make sure the backend is running before starting the Angular app.
