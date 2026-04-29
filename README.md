# Task Management System

Final project for Advanced Programming. The solution contains:

- `TaskManagementAPI`: ASP.NET Core Web API with Entity Framework Core, PostgreSQL, ASP.NET Identity, JWT authentication, role-based authorization, and Swagger.
- `TaskManagementMvc`: ASP.NET Core MVC frontend that consumes the API.

## Technologies

- .NET 8
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core Code First
- PostgreSQL
- ASP.NET Identity
- JWT Bearer authentication
- Dependency Injection / IoC
- Swagger / OpenAPI

## Running the Project

1. Make sure PostgreSQL is running.
2. Confirm the API connection string in `TaskManagementAPI/appsettings.json`:

   ```json
   "DefaultConnection": "Host=localhost;Port=5432;Database=TaskManagmentSystemAdvancedProgramming;Username=postgres;Password=0000"
   ```

3. Apply the database migration if the database is not already created:

   ```bash
   dotnet ef database update --project TaskManagementAPI
   ```

4. Run both projects from Visual Studio using the configured multiple-startup setup, or run them separately:

   ```bash
   dotnet run --project TaskManagementAPI
   dotnet run --project TaskManagementMvc
   ```

5. Open the MVC app:

   ```text
   http://localhost:5193
   ```

6. Open Swagger for API testing:

   ```text
   http://localhost:5199/swagger
   ```

## Default Ports

- API: `http://localhost:5199`
- MVC: `http://localhost:5193`

The MVC API base URL is configured in:

- `TaskManagementMvc/appsettings.json`
- `TaskManagementMvc/appsettings.Development.json`

## Roles

The system supports three roles:

- `Admin`
- `Manager`
- `Employee`

Roles are stored by ASP.NET Identity in the normal Identity role tables. Registration does not remove roles from the database; it only prevents users from choosing their own role.

Authorization rules:

- Newly registered users are always created as `Employee`.
- Only an `Admin` can promote or demote users to `Manager` or `Admin`.
- Employees can create tasks for themselves and view only tasks assigned to them.
- Managers can view all tasks and change task assignments.
- Admins can view, create, edit, assign, and delete all tasks.
- Any authenticated user can view categories.
- Only admins can create, update, or delete categories.
- Admins and managers can view users for task assignment.
- `Admin` can update user roles.

At least one admin account must exist before user role management can be used. In a company this account would be created by IT or seeded during deployment.

## API Endpoints

### Authentication

| Method | Endpoint | Description |
| --- | --- | --- |
| POST | `/api/Auth/register` | Register a user as Employee |
| POST | `/api/Auth/login` | Login and receive a JWT token |

### Tasks

| Method | Endpoint | Description | Authorization |
| --- | --- | --- | --- |
| GET | `/api/Tasks` | Get all visible tasks | Authenticated |
| GET | `/api/Tasks/{id}` | Get visible task by ID | Authenticated |
| POST | `/api/Tasks` | Create task | Admin, Employee |
| PUT | `/api/Tasks/{id}` | Full task update | Admin |
| PUT | `/api/Tasks/{id}/assignment` | Change task assignment | Admin, Manager |
| DELETE | `/api/Tasks/{id}` | Delete task | Admin |

### Categories

| Method | Endpoint | Description | Authorization |
| --- | --- | --- | --- |
| GET | `/api/Categories` | Get all categories | Authenticated |
| GET | `/api/Categories/{id}` | Get category by ID | Authenticated |
| POST | `/api/Categories` | Create category | Admin |
| PUT | `/api/Categories/{id}` | Update category | Admin |
| DELETE | `/api/Categories/{id}` | Delete category | Admin |

### Users

| Method | Endpoint | Description | Authorization |
| --- | --- | --- | --- |
| GET | `/api/Users` | Get users for task assignment | Admin, Manager |
| PUT | `/api/Users/{id}/role` | Update a user's role | Admin |

## Database Schema

The database is created using Entity Framework Core Code First migrations.

Main tables:

- `AspNetUsers`: Identity users with `FirstName`, `LastName`, `JobTitle`, and `CreatedAt`.
- `AspNetRoles`: Identity roles.
- `AspNetUserRoles`: user-role relationship.
- `Categories`: task categories.
- `TaskItems`: task records.

Main relationships:

- One `Category` has many `TaskItems`.
- One `ApplicationUser` can have many assigned `TaskItems`.
- Identity manages users, roles, claims, logins, and tokens.

## MVC Features

The MVC project consumes the API through `IHttpClientFactory` and stores the JWT token in session after login.

Implemented screens:

- Login
- Register
- Dashboard
- User management for admins
- Task list
- Task details
- Create task for employees and admins
- Full task editing for admins
- Task assignment updates for managers and admins
- Delete task for admins
- Category list
- Create, edit, and delete categories for admins

## Swagger Authentication

1. Register a user through `/api/Auth/register`.
2. Login through `/api/Auth/login`.
3. Copy the returned JWT token.
4. Click `Authorize` in Swagger.
5. Paste the token.
6. Test protected endpoints.

## Project Structure

```text
TaskManagementSolution
├── TaskManagementAPI
│   ├── Controllers
│   ├── Data
│   ├── DTOs
│   ├── Migrations
│   ├── Models
│   └── Services
└── TaskManagementMvc
    ├── Controllers
    ├── Services
    ├── ViewModels
    ├── Views
    └── wwwroot
```
