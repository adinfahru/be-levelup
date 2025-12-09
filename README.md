# LevelUp Backend API

LevelUp is an employee development management system that enables organizations to manage employee training and skill development through structured modules, enrollments, and progress tracking.

## 🎯 Project Overview

The LevelUp Backend provides a comprehensive REST API for managing:
- **User Authentication & Authorization** - Role-based access control (Admin, Manager, Employee)
- **Module Management** - Create and manage training modules with structured content items
- **Employee Enrollments** - Track employee progress through training modules
- **Progress Tracking** - Monitor completion of module items and track overall progress
- **Submission & Review** - Submit final work and manager review workflow
- **Dashboard Analytics** - Manager dashboards with employee and enrollment statistics

## 🏗️ Tech Stack

- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **Database**: SQL Server
- **Authentication**: JWT (JSON Web Tokens)
- **API Style**: RESTful

## 📋 Key Features

### Authentication & Authorization
- User registration and login with JWT tokens
- Role-based access control (RBAC) with three roles
- Password management and security features
- Token expiration and refresh mechanisms

### Module Management
- Create, read, update, and manage training modules
- Structure modules with ordered items/lessons
- Soft delete support for data safety
- Module status management (active/inactive)

### Employee Development
- Self-enrolled or assigned module enrollments
- Track progress through module items
- Provide evidence and feedback for completed items
- Pause and resume enrollments

### Submission & Review
- Submit final project work for manager review
- Manager feedback and approval workflow
- Support for multiple submission attempts
- Submission history tracking

### Manager Features
- Comprehensive dashboard with statistics
- Employee management and oversight
- Submission review and approval
- Employee enrollment tracking
- Performance analytics

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019+
- Git

### Installation

```bash
# Clone the repository
git clone https://github.com/adinfahru/level-up-project.git
cd be-levelup

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the API
dotnet run --project LevelUp.API
```

The API will be available at `https://localhost:7118`

### Database Setup

```bash
# Apply migrations (when available)
dotnet ef database update

# Or create schema from migration files
# See migrations folder for details
```

## 📚 API Documentation

Comprehensive API documentation is available in [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)

### Quick API Reference

| Domain | Method | Endpoint | Purpose |
|--------|--------|----------|---------|
| **Auth** | POST | `/auth/login` | User login |
| **Users** | GET | `/admin/users` | List all users |
| **Users** | POST | `/admin/users` | Create user |
| **Modules** | GET | `/modules` | List modules |
| **Modules** | POST | `/modules` | Create module |
| **Enrollments** | GET | `/enrollments` | List enrollments |
| **Enrollments** | POST | `/enrollments` | Create enrollment |
| **Submissions** | GET | `/submissions` | List submissions |
| **Dashboard** | GET | `/manager/dashboard` | Manager dashboard |

## 🗄️ Database Schema

The system uses 8 core tables:

- **Accounts** - User accounts and authentication
- **Employees** - Employee information with manager hierarchy
- **Positions** - Job position definitions
- **Modules** - Training module definitions
- **ModuleItems** - Individual lessons/items within modules
- **Enrollments** - Employee enrollment in modules
- **EnrollmentItems** - Tracking of completed module items
- **Submissions** - Final project submissions and reviews

See [API_DOCUMENTATION.md](./API_DOCUMENTATION.md#database-schema) for detailed schema information.

## 👥 User Roles

### Admin
- Manage user accounts (create, read, update, delete)
- Manage job positions
- System configuration

### Manager
- Create and manage training modules
- Submit final work on behalf of employees
- Review and approve submissions
- View employee enrollments and progress
- Manager dashboard with analytics

### Employee
- View available modules
- Enroll in modules
- Track personal progress
- Mark items as completed with evidence
- Resume paused enrollments

## 🔐 Authentication

All endpoints (except `/auth/login`) require Bearer token authentication:

```
Authorization: Bearer {jwt_token}
```

Tokens are obtained via the login endpoint and expire after 24 hours.

## 📦 Project Structure

### Overview Architecture Diagram

```
┌─────────────────────────────────────────────────────┐
│         Presentation Layer (Controllers)             │
│  - Request handling & response formatting           │
└─────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────┐
│      Application Layer (Services & DTOs)            │
│  - Business logic & data transformation            │
└─────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────┐
│      Domain Layer (Models & Entities)               │
│  - Core business objects & rules                   │
└─────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────┐
│   Infrastructure Layer (Data & Repositories)        │
│  - Database access & external services             │
└─────────────────────────────────────────────────────┘
```

### Detailed Directory Structure

```
LevelUp/
├── LevelUp.sln                          # Solution file
├── README.md                            # Project overview
├── API_DOCUMENTATION.md                 # API reference
│
└── LevelUp.API/                         # Main API Project
    │
    ├── 📁 Controllers/                  # Presentation Layer
    │   ├── AuthController.cs            # Authentication endpoints
    │   ├── AdminController.cs           # Admin endpoints (users, positions)
    │   ├── ModulesController.cs         # Module management endpoints
    │   ├── EnrollmentsController.cs     # Enrollment endpoints
    │   ├── SubmissionsController.cs     # Submission review endpoints
    │   └── DashboardController.cs       # Manager dashboard endpoints
    │
    ├── 📁 Models/                       # Domain Layer
    │   ├── Entities/
    │   │   ├── Account.cs               # User account entity
    │   │   ├── Employee.cs              # Employee entity
    │   │   ├── Position.cs              # Job position entity
    │   │   ├── Module.cs                # Training module entity
    │   │   ├── ModuleItem.cs            # Module item/lesson entity
    │   │   ├── Enrollment.cs            # Enrollment entity
    │   │   ├── EnrollmentItem.cs        # Enrollment progress entity
    │   │   └── Submission.cs            # Final submission entity
    │   │
    │   └── Enums/
    │       ├── UserRole.cs              # Admin, Manager, Employee
    │       ├── EnrollmentStatus.cs      # OnGoing, Paused, Completed
    │       └── SubmissionStatus.cs      # Pending, Approved, Rejected
    │
    ├── 📁 DTOs/                         # Application Layer - Data Transfer Objects
    │   ├── Auth/
    │   │   ├── LoginRequest.cs
    │   │   ├── LoginResponse.cs
    │   │   └── ChangePasswordRequest.cs
    │   │
    │   ├── Users/
    │   │   ├── CreateUserRequest.cs
    │   │   ├── UpdateUserRequest.cs
    │   │   └── UserResponse.cs
    │   │
    │   ├── Modules/
    │   │   ├── CreateModuleRequest.cs
    │   │   ├── UpdateModuleRequest.cs
    │   │   └── ModuleResponse.cs
    │   │
    │   ├── Enrollments/
    │   │   ├── CreateEnrollmentRequest.cs
    │   │   ├── EnrollmentResponse.cs
    │   │   └── EnrollmentProgressResponse.cs
    │   │
    │   └── Submissions/
    │       ├── SubmitWorkRequest.cs
    │       ├── ReviewSubmissionRequest.cs
    │       └── SubmissionResponse.cs
    │
    ├── 📁 Services/                     # Application Layer - Business Logic
    │   ├── Interfaces/
    │   │   ├── IAuthService.cs
    │   │   ├── IUserService.cs
    │   │   ├── IModuleService.cs
    │   │   ├── IEnrollmentService.cs
    │   │   ├── ISubmissionService.cs
    │   │   └── IDashboardService.cs
    │   │
    │   └── Implementations/
    │       ├── AuthService.cs           # Authentication logic
    │       ├── UserService.cs           # User management logic
    │       ├── ModuleService.cs         # Module management logic
    │       ├── EnrollmentService.cs     # Enrollment logic
    │       ├── SubmissionService.cs     # Submission logic
    │       └── DashboardService.cs      # Dashboard statistics logic
    │
    ├── 📁 Data/                         # Infrastructure Layer
    │   ├── ApplicationDbContext.cs      # EF Core DbContext
    │   ├── Configurations/              # EF Core entity configurations
    │   │   ├── AccountConfiguration.cs
    │   │   ├── EmployeeConfiguration.cs
    │   │   ├── EnrollmentConfiguration.cs
    │   │   └── SubmissionConfiguration.cs
    │   │
    │   └── Migrations/                  # Database migrations
    │       ├── 20251209000000_InitialCreate.cs
    │       ├── 20251209000001_AddIndexes.cs
    │       └── ...
    │
    ├── 📁 Repositories/                 # Infrastructure Layer - Data Access
    │   ├── Interfaces/
    │   │   ├── IRepository.cs           # Generic repository interface
    │   │   ├── IEnrollmentRepository.cs # Specific repository interfaces
    │   │   ├── IUserRepository.cs
    │   │   ├── IModuleRepository.cs
    │   │   └── IUnitOfWork.cs           # Transaction management
    │   │
    │   └── Implementations/
    │       ├── Repository.cs            # Generic repository implementation
    │       ├── EnrollmentRepository.cs
    │       ├── UserRepository.cs
    │       ├── ModuleRepository.cs
    │       └── UnitOfWork.cs
    │
    ├── 📁 Middleware/                   # Cross-cutting concerns
    │   ├── ErrorHandlingMiddleware.cs   # Global error handler
    │   ├── JwtMiddleware.cs             # Token validation (if needed)
    │   └── LoggingMiddleware.cs         # Request/response logging
    │
    ├── 📁 Filters/                      # Action filters & validators
    │   ├── ValidationFilter.cs          # Model validation
    │   ├── AuthorizationFilter.cs       # Authorization checks
    │   └── ExceptionFilter.cs           # Exception handling
    │
    ├── 📁 Exceptions/                   # Custom exceptions
    │   ├── NotFoundException.cs
    │   ├── InvalidOperationException.cs
    │   ├── DuplicateResourceException.cs
    │   └── UnauthorizedException.cs
    │
    ├── 📁 Utilities/                    # Helper functions
    │   ├── JwtTokenHelper.cs            # JWT token generation/validation
    │   ├── PasswordHasher.cs            # Password hashing/verification
    │   ├── ValidationHelper.cs          # Input validation helpers
    │   └── MappingProfiles.cs           # AutoMapper configurations
    │
    ├── 📁 Properties/
    │   └── launchSettings.json          # Launch profiles
    │
    ├── 📁 bin/                          # Compiled output
    ├── 📁 obj/                          # Build artifacts
    │
    ├── appsettings.json                 # Configuration (production)
    ├── appsettings.Development.json     # Configuration (development)
    ├── Program.cs                       # Application entry point & DI setup
    ├── LevelUp.API.csproj              # Project file
    └── LevelUp.API.http                # REST client requests (test)
```

### Layer Responsibilities

#### 🎯 Controllers (Presentation Layer)
**Responsibility:** Handle HTTP requests and responses
- Validate input parameters
- Call appropriate services
- Format responses
- Handle HTTP status codes

```

### File Naming Conventions

| Type | Convention | Example |
|------|-----------|---------|
| **Controllers** | `{Feature}Controller.cs` | `EnrollmentsController.cs` |
| **Services** | `I{Feature}Service.cs` + `{Feature}Service.cs` | `IEnrollmentService.cs`, `EnrollmentService.cs` |
| **Repositories** | `I{Entity}Repository.cs` + `{Entity}Repository.cs` | `IEnrollmentRepository.cs`, `EnrollmentRepository.cs` |
| **DTOs** | `{Action}{Entity}{Direction}.cs` | `CreateEnrollmentRequest.cs`, `EnrollmentResponse.cs` |
| **Entities** | `{Entity}.cs` | `Enrollment.cs` |
| **Enums** | `{Name}.cs` | `EnrollmentStatus.cs` |
| **Middleware** | `{Name}Middleware.cs` | `ErrorHandlingMiddleware.cs` |
| **Filters** | `{Name}Filter.cs` | `ValidationFilter.cs` |
| **Exceptions** | `{Name}Exception.cs` | `NotFoundException.cs` |

### Key Folders Explained

| Folder | Purpose | Contains |
|--------|---------|----------|
| **Controllers** | API endpoints | HTTP route handlers |
| **Models/Entities** | Core business objects | Database entities |
| **Models/Enums** | Enumeration types | Status, role constants |
| **DTOs** | Data contracts | Request/response objects |
| **Services** | Business logic | Use case implementations |
| **Data** | Database context | EF Core configuration |
| **Repositories** | Data access patterns | Generic CRUD operations |
| **Middleware** | Pipeline handlers | Cross-cutting concerns |
| **Filters** | Decorators | Validation, authorization |
| **Exceptions** | Custom errors | Domain-specific exceptions |
| **Utilities** | Helper functions | Reusable logic |

## 📝 API Response Format

### Success Response (2xx)
```json
{
  "success": true,
  "data": {
    // Response data
  },
  "message": "Operation successful"
}
```

### Error Response (4xx, 5xx)
```json
{
  "success": false,
  "error": {
    "code": "ERROR_CODE",
    "message": "Detailed error message"
  },
}
```

## 🔧 Configuration

Configuration is managed via `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LevelUp;..."
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "levelup-api",
    "Audience": "levelup-client",
    "ExpirationMinutes": 1440
  }
}
```

## 🐛 Common Issues

### Database Connection Fails
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure user has appropriate permissions

### JWT Token Errors
- Verify token is included in Authorization header
- Check token hasn't expired
- Ensure token is properly formatted with "Bearer " prefix

## 📖 Development Guidelines

- Follow C# coding conventions (PascalCase for public members)
- Use async/await for I/O operations
- Implement dependency injection for services

---

**Last Updated**: December 9, 2025  
**API Version**: v1  
**Base URL**: `https://api.levelup.local/api/v1`
