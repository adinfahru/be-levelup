# LevelUp API Documentation

## Table of Contents
1. [Overview](#overview)
2. [Authentication](#authentication)
3. [Response Format](#response-format)
4. [HTTP Status Codes](#http-status-codes)
5. [API Endpoints](#api-endpoints)
6. [Database Schema](#database-schema)
7. [Error Codes](#error-codes)
8. [Notes](#notes)

---

## Overview

LevelUp is an employee development management system that enables employees to enroll in training modules, track their progress, and submit final work for manager review. The system supports role-based access control with three primary roles: Admin, Manager, and Employee.

### Base URL

**Development:**
```
https://localhost:7118
```

**Production:**
```
https://api.levelup.local
```

### API Version
Current version: **v1**

### Routing Convention

All API endpoints follow the pattern:
```
{base_url}/api/v1/{resource}
```

**Important Notes for .NET Implementation:**
- Controllers should use `[Route("api/v1/{controller-name}")]` attribute
- Avoid using `[Route("api/[controller]")]` as it uses the class name (e.g., `ModulesController` → `/api/Modules`)
- Best practice: Explicitly define routes to maintain consistency between documentation and implementation
- Example: `[Route("api/v1/auth")]`, `[Route("api/v1/users")]`, `[Route("api/v1/modules")]`

**Full Endpoint Format:**
```
{base_url}/api/v1/{resource}/{action}
```

Example:
```
https://localhost:7118/api/v1/auth/login
https://localhost:7118/api/v1/users
https://localhost:7118/api/v1/modules
https://localhost:7118/api/v1/enrollments
```

---

## API Endpoints Overview

### Authentication
| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| POST | `/api/v1/auth/login` | User login | No | - |
| PUT | `/api/v1/auth/change-password` | Change password | Yes | All |

### User Management
| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/v1/users` | Get all users | Yes | Admin |
| POST | `/api/v1/users` | Create new user | Yes | Admin |
| GET | `/api/v1/users/{id}` | Get user by ID | Yes | Admin |
| PUT | `/api/v1/users/{id}` | Update user | Yes | Admin |
| DELETE | `/api/v1/users/{id}` | Delete user (soft) | Yes | Admin |

### Position Management
| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/v1/positions` | Get all positions | Yes | Admin |
| POST | `/api/v1/positions` | Create new position | Yes | Admin |
| GET | `/api/v1/positions/{id}` | Get position by ID | Yes | Admin |
| PUT | `/api/v1/positions/{id}` | Update position | Yes | Admin |
| DELETE | `/api/v1/positions/{id}` | Delete position (soft) | Yes | Admin |

### Module Management
| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/v1/modules` | Get all modules | Yes | Manager, Employee |
| POST | `/api/v1/modules` | Create new module | Yes | Manager |
| GET | `/api/v1/modules/{id}` | Get module details | Yes | Manager, Employee |
| PUT | `/api/v1/modules/{id}` | Update module | Yes | Manager |
| PATCH | `/api/v1/modules/{id}/status` | Update module status | Yes | Manager |

### Enrollment Management
| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/v1/enrollments` | Get all enrollments | Yes | Employee |
| POST | `/api/v1/enrollments` | Create enrollment | Yes | Employee |
| GET | `/api/v1/enrollments/current` | Get current enrollment | Yes | Employee |
| GET | `/api/v1/enrollments/history` | Get enrollment history | Yes | Employee |
| POST | `/api/v1/enrollments/{id}/resume` | Resume enrollment | Yes | Employee |
| GET | `/api/v1/enrollments/{id}/progress` | Get enrollment progress | Yes | Employee |
| POST | `/api/v1/enrollments/{id}/items` | Mark item as completed | Yes | Employee |
| POST | `/api/v1/enrollments/{id}/submit` | Submit final work | Yes | Employee |

### Submission Management
| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/v1/submissions` | Get all submissions | Yes | Manager |
| POST | `/api/v1/submissions/{id}/review` | Review submission | Yes | Manager |

### Manager Dashboard
| Method | Endpoint | Description | Auth Required | Role |
|--------|----------|-------------|---------------|------|
| GET | `/api/v1/manager/dashboard` | Get dashboard summary | Yes | Manager |
| GET | `/api/v1/manager/employees` | Get managed employees | Yes | Manager |
| GET | `/api/v1/manager/employees/{id}/detail` | Get employee details | Yes | Manager |
| PATCH | `/api/v1/manager/employees/{id}/status` | Update employee status | Yes | Manager |

---

## Authentication

All endpoints (except `/auth/login`) require Bearer token authentication via the `Authorization` header:

```
Authorization: Bearer {jwt_token}
```

**Token Details:**
- Tokens are obtained through the login endpoint
- JWT format (JSON Web Token)
- Expiration: 24 hours
- Issued by: LevelUp Auth Service

**Example:**
```bash
curl -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
  https://localhost:7118/api/v1/enrollments
```

---

## Response Format

### Success Response (2xx)

```json
{
  "success": true,
  "data": {
    // Response data varies by endpoint
  },
  "message": "Operation successful"
}
```

**Fields:**
- `success` (boolean): Always true for successful responses
- `data` (object): Response payload (varies by endpoint)
- `message` (string): Optional human-readable message

### Error Response (4xx, 5xx)

```json
{
  "success": false,
  "error": {
    "code": "ERROR_CODE",
    "message": "Detailed error message"
  },
  "timestamp": "2025-12-09T10:30:00Z"
}
```

**Fields:**
- `success` (boolean): Always false for error responses
- `error` (object): Error details
  - `code` (string): Machine-readable error code
  - `message` (string): Human-readable error message
- `timestamp` (string): ISO 8601 formatted error timestamp

---

## HTTP Status Codes

| Code | Name | Description |
|------|------|-------------|
| 200 | OK | Request successful, response data included |
| 201 | Created | Resource created successfully |
| 204 | No Content | Request successful, no content to return |
| 400 | Bad Request | Invalid input data or validation failed |
| 401 | Unauthorized | Missing or invalid authentication token |
| 403 | Forbidden | Insufficient permissions for this resource |
| 404 | Not Found | Resource does not exist |
| 409 | Conflict | Resource already exists or constraint violation |
| 500 | Internal Server Error | Server error occurred |

---

## API Endpoints

### Authentication

#### Login
**POST** `/api/v1/auth/login`

Authenticate user and receive JWT token.

**Authorization:** None required

**Full URL:** `https://localhost:7118/api/v1/auth/login`

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "securePassword123"
}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "user@example.com",
      "role": "Employee",
      "firstName": "John",
      "lastName": "Doe"
    }
  }
}
```

**Error Responses:**

```json
// 401 - Invalid credentials
{
  "success": false,
  "error": {
    "code": "INVALID_CREDENTIALS",
    "message": "Email or password is incorrect"
  }
}
```

```json
// 400 - Validation failed
{
  "success": false,
  "error": {
    "code": "INVALID_INPUT",
    "message": "Email is required"
  }
}
```

---

#### Change Password
**PUT** `/api/v1/auth/change-password`

Change authenticated user's password.

**Authorization:** Required (Bearer token)

**Full URL:** `https://localhost:7118/api/v1/auth/change-password`

**Request Body:**
```json
{
  "currentPassword": "oldPassword123",
  "newPassword": "newPassword456"
}
```

**Response (200):**
```json
{
  "success": true,
  "message": "Password changed successfully"
}
```

**Error Responses:**

```json
// 400 - Current password incorrect
{
  "success": false,
  "error": {
    "code": "INVALID_INPUT",
    "message": "Current password is incorrect"
  }
}
```

---

### User Management (Admin)

#### Get All Users
**GET** `/api/v1/users`

Retrieve all user accounts with pagination and filtering.

**Authorization:** Admin only

**Full URL:** `https://localhost:7118/api/v1/users`

**Query Parameters:**
| Parameter | Type | Default | Required | Description |
|-----------|------|---------|----------|-------------|
| page | integer | 1 | No | Page number for pagination |
| limit | integer | 10 | No | Items per page (max: 100) |
| role | string | - | No | Filter by role (Admin, Manager, Employee) |
| search | string | - | No | Search by email or name |
| isActive | boolean | - | No | Filter by active status |

**Example Request:**
```
GET /api/v1/users?page=1&limit=10&role=Employee&isActive=true
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440000",
        "email": "john@example.com",
        "role": "Employee",
        "firstName": "John",
        "lastName": "Doe",
        "isActive": true,
        "createdAt": "2025-12-01T08:00:00Z",
        "updatedAt": "2025-12-09T10:30:00Z"
      },
      {
        "id": "550e8400-e29b-41d4-a716-446655440001",
        "email": "jane@example.com",
        "role": "Manager",
        "firstName": "Jane",
        "lastName": "Smith",
        "isActive": true,
        "createdAt": "2025-12-02T08:00:00Z",
        "updatedAt": "2025-12-08T14:20:00Z"
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 10,
      "total": 25,
      "totalPages": 3
    }
  }
}
```

---

#### Create User
**POST** `/api/v1/users`

Create a new user account.

**Authorization:** Admin only

**Full URL:** `https://localhost:7118/api/v1/users`

**Request Body:**
```json
{
  "email": "newuser@example.com",
  "password": "initialPassword123",
  "role": "Employee",
  "firstName": "Jane",
  "lastName": "Smith"
}
```

**Field Validation:**
- `email`: Required, must be valid email format, must be unique
- `password`: Required, minimum 8 characters, must contain uppercase, lowercase, number
- `role`: Required, must be one of: Admin, Manager, Employee
- `firstName`: Required, maximum 100 characters
- `lastName`: Required, maximum 100 characters

**Response (201):**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "email": "newuser@example.com",
    "role": "Employee",
    "firstName": "Jane",
    "lastName": "Smith",
    "isActive": true
  },
  "message": "User created successfully"
}
```

**Error Responses:**

```json
// 409 - Email already exists
{
  "success": false,
  "error": {
    "code": "DUPLICATE_EMAIL",
    "message": "Email already registered"
  }
}
```

---

#### Update User
**PUT** `/api/v1/users/{id}`

Update user account details.

**Authorization:** Admin only

**Full URL:** `https://localhost:7118/api/v1/users/{id}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | uuid | User ID |

**Request Body:**
```json
{
  "email": "updated@example.com",
  "role": "Manager",
  "isActive": true,
  "firstName": "Jane",
  "lastName": "Smith"
}
```

**Note:** Only include fields to be updated (partial update supported)

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "email": "updated@example.com",
    "role": "Manager",
    "firstName": "Jane",
    "lastName": "Smith",
    "isActive": true
  },
  "message": "User updated successfully"
}
```

---

#### Delete User
**DELETE** `/api/v1/users/{id}`

Soft delete user account (deactivate).

**Authorization:** Admin only

**Full URL:** `https://localhost:7118/api/v1/users/{id}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | uuid | User ID |

**Response (204):** No content

**Note:** User record is not deleted from database, `is_active` flag is set to false

---

### Position Management (Admin)

#### Get All Positions
**GET** `/api/v1/positions`

Retrieve all job positions.

**Authorization:** Admin only

**Full URL:** `https://localhost:7118/api/v1/positions`

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number |
| limit | integer | 10 | Items per page |
| isActive | boolean | true | Filter by active status |

**Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "660e8400-e29b-41d4-a716-446655440000",
        "title": "Software Engineer",
        "isActive": true
      },
      {
        "id": "660e8400-e29b-41d4-a716-446655440001",
        "title": "Senior Developer",
        "isActive": true
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 10,
      "total": 5,
      "totalPages": 1
    }
  }
}
```

---

#### Create Position
**POST** `/api/v1/positions`

Create a new job position.

**Authorization:** Admin only

**Full URL:** `https://localhost:7118/api/v1/positions`

**Request Body:**
```json
{
  "title": "Senior Developer"
}
```

**Field Validation:**
- `title`: Required, maximum 100 characters, must be unique

**Response (201):**
```json
{
  "success": true,
  "data": {
    "id": "660e8400-e29b-41d4-a716-446655440002",
    "title": "Senior Developer",
    "isActive": true
  }
}
```

---

#### Update Position
**PUT** `/api/v1/positions/{id}`

Update job position details.

**Authorization:** Admin only

**Full URL:** `https://localhost:7118/api/v1/positions/{id}`

**Request Body:**
```json
{
  "title": "Lead Developer"
}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "660e8400-e29b-41d4-a716-446655440002",
    "title": "Lead Developer",
    "isActive": true
  }
}
```

---

#### Delete Position
**DELETE** `/api/v1/positions/{id}`

Deactivate a job position.

**Authorization:** Admin only

**Full URL:** `https://localhost:7118/api/v1/positions/{id}`

**Response (204):** No content

---

### Module Management (Manager)

#### Get All Modules
**GET** `/api/v1/modules`

Retrieve all training modules with optional filters.

**Authorization:** Manager, Employee

**Full URL:** `https://localhost:7118/api/v1/modules`

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number |
| limit | integer | 10 | Items per page |
| isActive | boolean | true | Filter by active status |
| createdBy | uuid | - | Filter by creator ID |

**Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "770e8400-e29b-41d4-a716-446655440000",
        "title": "Advanced C# Programming",
        "description": "Learn advanced C# concepts and best practices",
        "estimatedDays": 14,
        "isActive": true,
        "createdBy": "550e8400-e29b-41d4-a716-446655440000",
        "createdAt": "2025-12-01T08:00:00Z",
        "itemCount": 5
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 10,
      "total": 8,
      "totalPages": 1
    }
  }
}
```

---

#### Get Module Details
**GET** `/api/v1/modules/{id}`

Retrieve detailed information about a specific module including all items.

**Authorization:** Manager, Employee

**Full URL:** `https://localhost:7118/api/v1/modules/{id}`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | uuid | Module ID |

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "770e8400-e29b-41d4-a716-446655440000",
    "title": "Advanced C# Programming",
    "description": "Learn advanced C# concepts and best practices",
    "estimatedDays": 14,
    "isActive": true,
    "createdBy": "550e8400-e29b-41d4-a716-446655440000",
    "createdAt": "2025-12-01T08:00:00Z",
    "items": [
      {
        "id": "880e8400-e29b-41d4-a716-446655440000",
        "moduleId": "770e8400-e29b-41d4-a716-446655440000",
        "title": "Async/Await Deep Dive",
        "orderIndex": 1,
        "description": "Understanding async programming patterns",
        "url": "https://example.com/resources/async",
        "isFinalSubmission": false
      },
      {
        "id": "880e8400-e29b-41d4-a716-446655440001",
        "moduleId": "770e8400-e29b-41d4-a716-446655440000",
        "title": "Final Project",
        "orderIndex": 5,
        "description": "Build a complete async application",
        "url": "https://example.com/resources/final-project",
        "isFinalSubmission": true
      }
    ]
  }
}
```

---

#### Create Module
**POST** `/api/v1/modules`

Create a new training module with items.

**Authorization:** Manager only

**Full URL:** `https://localhost:7118/api/v1/modules`

**Request Body:**
```json
{
  "title": "Microservices Architecture",
  "description": "Building scalable microservices with .NET",
  "estimatedDays": 21,
  "items": [
    {
      "title": "Introduction to Microservices",
      "orderIndex": 1,
      "description": "Core concepts and patterns",
      "url": "https://example.com/microservices-intro",
      "isFinalSubmission": false
    },
    {
      "title": "Implementing Services",
      "orderIndex": 2,
      "description": "Build your first microservice",
      "url": "https://example.com/implement-service",
      "isFinalSubmission": false
    }
  ]
}
```

**Field Validation:**
- `title`: Required, maximum 200 characters
- `description`: Optional, maximum 2000 characters
- `estimatedDays`: Required, minimum 1, maximum 365
- `items`: Optional array of module items
  - `title`: Required, maximum 200 characters
  - `orderIndex`: Required, must be sequential starting from 1
  - `description`: Optional, maximum 1000 characters
  - `url`: Optional, must be valid URL format
  - `isFinalSubmission`: Optional, boolean

**Response (201):**
```json
{
  "success": true,
  "data": {
    "id": "770e8400-e29b-41d4-a716-446655440001",
    "title": "Microservices Architecture",
    "description": "Building scalable microservices with .NET",
    "estimatedDays": 21,
    "isActive": true,
    "createdBy": "550e8400-e29b-41d4-a716-446655440000",
    "itemCount": 2
  }
}
```

---

#### Update Module
**PUT** `/api/v1/modules/{id}`

Update module details.

**Authorization:** Manager only

**Full URL:** `https://localhost:7118/api/v1/modules/{id}`

**Request Body:**
```json
{
  "title": "Microservices Architecture - Advanced",
  "description": "Building scalable microservices with .NET and Cloud",
  "estimatedDays": 28
}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "770e8400-e29b-41d4-a716-446655440001",
    "title": "Microservices Architecture - Advanced",
    "description": "Building scalable microservices with .NET and Cloud",
    "estimatedDays": 28,
    "isActive": true
  }
}
```

---

#### Update Module Status
**PATCH** `/api/v1/modules/{id}/status`

Activate or deactivate a module.

**Authorization:** Manager only

**Full URL:** `https://localhost:7118/api/v1/modules/{id}/status`

**Request Body:**
```json
{
  "isActive": false
}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "770e8400-e29b-41d4-a716-446655440001",
    "isActive": false
  },
  "message": "Module status updated"
}
```

---

### Enrollment Management (Employee)

#### Get All Enrollments
**GET** `/api/v1/enrollments`

Retrieve all enrollments for the authenticated user.

**Authorization:** Required (Bearer token)

**Full URL:** `https://localhost:7118/api/v1/enrollments`

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number |
| limit | integer | 10 | Items per page |
| status | string | - | Filter by status (OnGoing, Paused, Completed) |

**Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "990e8400-e29b-41d4-a716-446655440000",
        "moduleId": "770e8400-e29b-41d4-a716-446655440000",
        "moduleName": "Advanced C# Programming",
        "startDate": "2025-12-01T08:00:00Z",
        "targetDate": "2025-12-15T08:00:00Z",
        "completedDate": null,
        "status": "OnGoing",
        "currentProgress": 35.5
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 10,
      "total": 3,
      "totalPages": 1
    }
  }
}
```

---

#### Get Current Enrollment
**GET** `/api/v1/enrollments/current`

Get the employee's currently active enrollment.

**Authorization:** Required (Bearer token)

**Full URL:** `https://localhost:7118/api/v1/enrollments/current`

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "990e8400-e29b-41d4-a716-446655440000",
    "moduleId": "770e8400-e29b-41d4-a716-446655440000",
    "moduleName": "Advanced C# Programming",
    "startDate": "2025-12-01T08:00:00Z",
    "targetDate": "2025-12-15T08:00:00Z",
    "status": "OnGoing",
    "currentProgress": 35.5
  }
}
```

**Error Response:**

```json
// 404 - No active enrollment
{
  "success": false,
  "error": {
    "code": "NOT_FOUND",
    "message": "No active enrollment found"
  }
}
```

---

#### Get Enrollment History
**GET** `/api/v1/enrollments/history`

Get employee's completed and paused enrollments.

**Authorization:** Required (Bearer token)

**Full URL:** `https://localhost:7118/api/v1/enrollments/history`

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number |
| limit | integer | 10 | Items per page |

**Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "990e8400-e29b-41d4-a716-446655440001",
        "moduleId": "770e8400-e29b-41d4-a716-446655440000",
        "moduleName": "Basic Python",
        "startDate": "2025-11-01T08:00:00Z",
        "completedDate": "2025-11-20T15:30:00Z",
        "status": "Completed",
        "currentProgress": 100
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 10,
      "total": 2,
      "totalPages": 1
    }
  }
}
```

---

#### Create Enrollment
**POST** `/api/v1/enrollments`

Enroll in a module.

**Authorization:** Required (Bearer token)

**Full URL:** `https://localhost:7118/api/v1/enrollments`

**Request Body:**
```json
{
  "moduleId": "770e8400-e29b-41d4-a716-446655440000"
}
```

**Field Validation:**
- `moduleId`: Required, must be valid module ID and active

**Response (201):**
```json
{
  "success": true,
  "data": {
    "id": "990e8400-e29b-41d4-a716-446655440002",
    "moduleId": "770e8400-e29b-41d4-a716-446655440000",
    "startDate": "2025-12-09T10:30:00Z",
    "targetDate": "2025-12-23T10:30:00Z",
    "status": "OnGoing",
    "currentProgress": 0
  },
  "message": "Enrollment created successfully"
}
```

**Error Response:**

```json
// 400 - Already enrolled
{
  "success": false,
  "error": {
    "code": "INVALID_INPUT",
    "message": "Already enrolled in this module"
  }
}
```

---

#### Resume Enrollment
**POST** `/api/v1/enrollments/{id}/resume`

Resume a paused enrollment.

**Authorization:** Required (Bearer token)

**Full URL:** `https://localhost:7118/api/v1/enrollments/{id}/resume`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | uuid | Enrollment ID |

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "990e8400-e29b-41d4-a716-446655440000",
    "status": "OnGoing"
  },
  "message": "Enrollment resumed successfully"
}
```

---

### Progress & Checklist (Employee)

#### Add Enrollment Item
**POST** `/api/v1/enrollments/{id}/items`

Mark a module item as completed.

**Authorization:** Required (Bearer token)

**Full URL:** `https://localhost:7118/api/v1/enrollments/{id}/items`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | uuid | Enrollment ID |

**Request Body:**
```json
{
  "moduleItemId": "880e8400-e29b-41d4-a716-446655440000",
  "feedback": "Successfully completed the async programming exercise",
  "evidenceUrl": "https://github.com/user/async-project"
}
```

**Field Validation:**
- `moduleItemId`: Required, must be valid item ID
- `feedback`: Optional, maximum 1000 characters
- `evidenceUrl`: Optional, must be valid URL format

**Response (201):**
```json
{
  "success": true,
  "data": {
    "id": "aa0e8400-e29b-41d4-a716-446655440000",
    "enrollmentId": "990e8400-e29b-41d4-a716-446655440000",
    "moduleItemId": "880e8400-e29b-41d4-a716-446655440000",
    "isCompleted": true,
    "completedAt": "2025-12-09T10:30:00Z"
  },
  "message": "Item marked as completed"
}
```

---

#### Get Enrollment Progress
**GET** `/api/v1/enrollments/{id}/progress`

Get detailed progress for an enrollment.

**Authorization:** Required (Bearer token)

**Full URL:** `https://localhost:7118/api/v1/enrollments/{id}/progress`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | uuid | Enrollment ID |

**Response (200):**
```json
{
  "success": true,
  "data": {
    "enrollmentId": "990e8400-e29b-41d4-a716-446655440000",
    "moduleName": "Advanced C# Programming",
    "currentProgress": 60,
    "totalItems": 5,
    "completedItems": 3,
    "items": [
      {
        "id": "880e8400-e29b-41d4-a716-446655440000",
        "title": "Async/Await Deep Dive",
        "orderIndex": 1,
        "isCompleted": true,
        "isFinalSubmission": false,
        "completedAt": "2025-12-05T14:00:00Z"
      },
      {
        "id": "880e8400-e29b-41d4-a716-446655440001",
        "title": "Final Project",
        "orderIndex": 5,
        "isCompleted": false,
        "isFinalSubmission": true,
        "completedAt": null
      }
    ]
  }
}
```

---

### Final Submission & Review (Manager)

#### Submit Final Work
**POST** `/api/v1/enrollments/{id}/submit`

Submit final work for an enrollment.

**Authorization:** Manager only

**Full URL:** `https://localhost:7118/api/v1/enrollments/{id}/submit`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | uuid | Enrollment ID |

**Request Body:**
```json
{
  "submissionUrl": "https://github.com/user/final-project",
  "notes": "Project completed with all requirements met"
}
```

**Field Validation:**
- `submissionUrl`: Required, must be valid URL
- `notes`: Optional, maximum 2000 characters

**Response (201):**
```json
{
  "success": true,
  "data": {
    "id": "bb0e8400-e29b-41d4-a716-446655440000",
    "enrollmentId": "990e8400-e29b-41d4-a716-446655440000",
    "submissionUrl": "https://github.com/user/final-project",
    "notes": "Project completed with all requirements met",
    "status": "Pending",
    "createdAt": "2025-12-09T10:30:00Z"
  }
}
```

---

#### Get All Submissions
**GET** `/api/v1/submissions`

Retrieve all submissions for review.

**Authorization:** Manager only

**Full URL:** `https://localhost:7118/api/v1/submissions`

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number |
| limit | integer | 10 | Items per page |
| status | string | Pending | Filter by status (Pending, Approved, Rejected) |

**Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "bb0e8400-e29b-41d4-a716-446655440000",
        "enrollmentId": "990e8400-e29b-41d4-a716-446655440000",
        "employeeName": "John Doe",
        "moduleName": "Advanced C# Programming",
        "submissionUrl": "https://github.com/user/final-project",
        "status": "Pending",
        "createdAt": "2025-12-09T10:30:00Z",
        "reviewedAt": null
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 10,
      "total": 5,
      "totalPages": 1
    }
  }
}
```

---

#### Review Submission
**POST** `/api/v1/submissions/{id}/review`

Review and approve/reject a submission.

**Authorization:** Manager only

**Full URL:** `https://localhost:7118/api/v1/submissions/{id}/review`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | uuid | Submission ID |

**Request Body:**
```json
{
  "status": "Approved",
  "feedback": "Excellent work! All requirements met and code quality is outstanding."
}
```

**Field Validation:**
- `status`: Required, must be one of: Approved, Rejected
- `feedback`: Optional, maximum 2000 characters

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "bb0e8400-e29b-41d4-a716-446655440000",
    "enrollmentId": "990e8400-e29b-41d4-a716-446655440000",
    "status": "Approved",
    "managerFeedback": "Excellent work! All requirements met and code quality is outstanding.",
    "reviewedAt": "2025-12-09T11:00:00Z"
  },
  "message": "Submission reviewed successfully"
}
```

---

### Manager Dashboard (Manager)

#### Get Dashboard Summary
**GET** `/api/v1/manager/dashboard`

Get manager's dashboard with summary statistics.

**Authorization:** Manager only

**Full URL:** `https://localhost:7118/api/v1/manager/dashboard`

**Response (200):**
```json
{
  "success": true,
  "data": {
    "totalEmployees": 15,
    "activeEnrollments": 8,
    "completedEnrollments": 12,
    "pendingSubmissions": 3,
    "enrollmentsByStatus": {
      "onGoing": 8,
      "paused": 2,
      "completed": 12
    },
    "submissionsByStatus": {
      "pending": 3,
      "approved": 10,
      "rejected": 1
    }
  }
}
```

---

#### Get Managed Employees
**GET** `/api/v1/manager/employees`

Get list of employees managed by this manager.

**Authorization:** Manager only

**Full URL:** `https://localhost:7118/api/v1/manager/employees`

**Query Parameters:**
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| page | integer | 1 | Page number |
| limit | integer | 10 | Items per page |
| isIdle | boolean | - | Filter by idle status |

**Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "cc0e8400-e29b-41d4-a716-446655440000",
        "firstName": "John",
        "lastName": "Doe",
        "email": "john@example.com",
        "positionTitle": "Software Engineer",
        "isIdle": false,
        "createdAt": "2025-10-15T08:00:00Z"
      }
    ],
    "pagination": {
      "page": 1,
      "limit": 10,
      "total": 15,
      "totalPages": 2
    }
  }
}
```

---

#### Get Employee Details
**GET** `/api/v1/manager/employees/{id}/detail`

Get detailed information about an employee including their enrollments and progress.

**Authorization:** Manager only

**Full URL:** `https://localhost:7118/api/v1/manager/employees/{id}/detail`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | uuid | Employee ID |

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "cc0e8400-e29b-41d4-a716-446655440000",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "positionTitle": "Software Engineer",
    "isIdle": false,
    "createdAt": "2025-10-15T08:00:00Z",
    "enrollments": [
      {
        "id": "990e8400-e29b-41d4-a716-446655440000",
        "moduleName": "Advanced C# Programming",
        "status": "OnGoing",
        "currentProgress": 60,
        "startDate": "2025-12-01T08:00:00Z",
        "targetDate": "2025-12-15T08:00:00Z"
      }
    ],
    "submissionCount": 5,
    "approvedSubmissions": 4,
    "rejectedSubmissions": 1
  }
}
```

---

#### Update Employee Status
**PATCH** `/api/v1/manager/employees/{id}/status`

Update employee's idle status.

**Authorization:** Manager only

**Full URL:** `https://localhost:7118/api/v1/manager/employees/{id}/status`

**Path Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| id | uuid | Employee ID |

**Request Body:**
```json
{
  "isIdle": true
}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": "cc0e8400-e29b-41d4-a716-446655440000",
    "isIdle": true
  },
  "message": "Employee status updated"
}
```

---

## Database Schema

### Enums

```
UserRole: Admin | Manager | Employee
EnrollmentStatus: OnGoing | Paused | Completed
SubmissionStatus: Pending | Approved | Rejected
```

### Tables

#### Accounts (Authentication Domain)
| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| id | UUID | PK, Default: newid() | Unique account identifier |
| email | VARCHAR | Unique, Not Null | User email address |
| password | VARCHAR | Not Null | Hashed password |
| role | UserRole | Not Null | User role (Admin, Manager, Employee) |
| otp | INT | Nullable | One-time password for 2FA |
| is_active | BOOLEAN | Default: true | Account status (true = active, false = deactivated) |
| created_at | TIMESTAMP | Default: now() | Creation timestamp |
| updated_at | TIMESTAMP | Nullable | Last update timestamp |

**Indexes:**
- Primary Key: id
- Unique Index: email

---

#### Employees (Business Domain)
| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| id | UUID | PK, Default: newid() | Unique employee identifier |
| account_id | UUID | FK (Accounts.id), Unique | Reference to account |
| manager_id | UUID | FK (Employees.id), Nullable | Self-reference to manager |
| first_name | VARCHAR | Not Null | Employee first name |
| last_name | VARCHAR | Not Null | Employee last name |
| position_id | UUID | FK (Positions.id) | Reference to position |
| is_idle | BOOLEAN | Default: true | Employee availability status |
| created_at | TIMESTAMP | Default: now() | Creation timestamp |
| updated_at | TIMESTAMP | Nullable | Last update timestamp |

**Indexes:**
- Primary Key: id
- Unique Index: account_id
- Foreign Key: manager_id (self-reference)
- Foreign Key: position_id

---

#### Positions
| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| id | UUID | PK, Default: newid() | Unique position identifier |
| title | VARCHAR | Not Null | Job position title |
| is_active | BOOLEAN | Default: true | Position availability |

**Indexes:**
- Primary Key: id

---

#### Modules
| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| id | UUID | PK, Default: newid() | Unique module identifier |
| title | VARCHAR | Not Null | Module title |
| description | TEXT | Nullable | Module description |
| estimated_days | INT | Not Null | Estimated duration in days (1-365) |
| is_active | BOOLEAN | Default: true | Module availability |
| created_by | UUID | FK (Accounts.id) | Manager who created module |
| created_at | TIMESTAMP | Default: now() | Creation timestamp |
| updated_at | TIMESTAMP | Nullable | Last update timestamp |
| deleted_at | TIMESTAMP | Nullable | Soft delete timestamp (null = not deleted) |

**Indexes:**
- Primary Key: id
- Foreign Key: created_by
- Index: is_active (for filtering)

---

#### ModuleItems
| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| id | UUID | PK, Default: newid() | Unique item identifier |
| module_id | UUID | FK (Modules.id) | Reference to module |
| title | VARCHAR | Not Null | Item title |
| order_index | INT | Not Null | Display order (must be sequential, 1-based) |
| description | VARCHAR | Nullable | Item description |
| url | VARCHAR | Nullable | Resource URL |
| is_final_submission | BOOLEAN | Default: false | Final project submission indicator |

**Indexes:**
- Primary Key: id
- Foreign Key: module_id
- Index: (module_id, order_index) for ordering

---

#### Enrollments (Core Logic)
| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| id | UUID | PK, Default: newid() | Unique enrollment identifier |
| account_id | UUID | FK (Accounts.id) | Reference to employee account |
| module_id | UUID | FK (Modules.id) | Reference to module |
| start_date | TIMESTAMP | Default: now() | Enrollment start date |
| target_date | TIMESTAMP | Nullable | Target completion date |
| completed_date | TIMESTAMP | Nullable | Actual completion date |
| status | EnrollmentStatus | Default: OnGoing | Current enrollment status |
| current_progress | FLOAT | Default: 0 | Progress percentage (0-100) |
| created_at | TIMESTAMP | Default: now() | Creation timestamp |
| updated_at | TIMESTAMP | Nullable | Last update timestamp |

**Indexes:**
- Primary Key: id
- Foreign Key: account_id
- Foreign Key: module_id
- Index: (account_id, status) for filtering

---

#### EnrollmentItems
| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| id | UUID | PK, Default: newid() | Unique item progress identifier |
| enrollment_id | UUID | FK (Enrollments.id) | Reference to enrollment |
| module_item_id | UUID | FK (ModuleItems.id) | Reference to module item |
| is_completed | BOOLEAN | Default: false | Completion status |
| feedback | VARCHAR | Nullable | User feedback or notes |
| evidence_url | VARCHAR | Nullable | URL to evidence/work submission |
| completed_at | TIMESTAMP | Nullable | Completion timestamp |

**Indexes:**
- Primary Key: id
- Foreign Key: enrollment_id
- Foreign Key: module_item_id
- Unique Index: (enrollment_id, module_item_id) - Prevents completing same item twice

---

#### Submissions (History Logic)
| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| id | UUID | PK, Default: newid() | Unique submission identifier |
| enrollment_id | UUID | FK (Enrollments.id) | Reference to enrollment |
| submission_url | VARCHAR | Not Null | URL to submitted work |
| notes | TEXT | Nullable | Employee notes during submission |
| status | SubmissionStatus | Default: Pending | Submission review status |
| manager_feedback | TEXT | Nullable | Manager's feedback on submission |
| reviewed_at | TIMESTAMP | Nullable | Review completion timestamp |
| created_at | TIMESTAMP | Default: now() | Submission timestamp |

**Indexes:**
- Primary Key: id
- Foreign Key: enrollment_id
- Index: status (for filtering)

---

### Entity Relationships

```
┌─────────────┐
│  Accounts   │
└─────────────┘
      │ (1)
      ├──────(N)──→ Employees (account_id FK)
      ├──────(N)──→ Modules (created_by FK)
      └──────(N)──→ Enrollments (account_id FK)

┌─────────────┐
│ Employees   │
└─────────────┘
      │ (1)
      ├──────(N)──→ Employees (manager_id FK - self-reference)
      ├──────(1)──→ Positions (position_id FK)
      └──────(1)──→ Accounts (account_id FK)

┌─────────────┐
│  Positions  │
└─────────────┘
      │ (1)
      └──────(N)──→ Employees (position_id FK)

┌─────────────┐
│   Modules   │
└─────────────┘
      │ (1)
      ├──────(N)──→ ModuleItems (module_id FK)
      └──────(N)──→ Enrollments (module_id FK)

┌──────────────────┐
│   ModuleItems    │
└──────────────────┘
      │ (1)
      └──────(N)──→ EnrollmentItems (module_item_id FK)

┌──────────────────┐
│   Enrollments    │
└──────────────────┘
      │ (1)
      ├──────(N)──→ EnrollmentItems (enrollment_id FK)
      └──────(N)──→ Submissions (enrollment_id FK)

┌──────────────────┐
│ EnrollmentItems  │
└──────────────────┘

┌──────────────────┐
│   Submissions    │
└──────────────────┘
```

---

## Error Codes

| Code | HTTP | Description | Solution |
|------|------|-------------|----------|
| INVALID_CREDENTIALS | 401 | Email or password is incorrect | Verify credentials and try again |
| TOKEN_EXPIRED | 401 | JWT token has expired | Request new token via login |
| UNAUTHORIZED | 401 | Missing authentication token | Include Bearer token in Authorization header |
| FORBIDDEN | 403 | User lacks required permissions | Check user role for this operation |
| NOT_FOUND | 404 | Resource does not exist | Verify resource ID exists |
| DUPLICATE_EMAIL | 409 | Email already registered | Use different email or reset password |
| DUPLICATE_POSITION | 409 | Position title already exists | Use different position title |
| INVALID_INPUT | 400 | Request validation failed | Check request body format and required fields |
| MODULE_IN_USE | 409 | Cannot delete module with active enrollments | Deactivate module or complete enrollments first |
| ENROLLMENT_COMPLETE | 400 | Cannot modify completed enrollment | Archive completed enrollment instead |
| INTERNAL_ERROR | 500 | Server error occurred | Contact support or check server logs |

---

## Notes

### Timestamps
- All timestamps are in ISO 8601 format (UTC)
- Example: `2025-12-09T10:30:00Z`
- Automatically set by database on creation/update

### Pagination
- Default values: page=1, limit=10
- Maximum limit: 100 items per page
- Response includes total count and total pages
- Use page number and limit to navigate results

### Soft Deletes
- **Modules**: Use `deleted_at` timestamp (null = active)
- **Accounts**: Use `is_active` boolean flag

### Progress Calculation
```
current_progress = (completed_items / total_items) × 100
```

### Target Date Calculation
```
target_date = start_date + (module.estimated_days × 24 hours)
```

### Module Items
- Must have sequential order_index starting from 1
- Only one item per module can have `isFinalSubmission = true`
- Items define the structure of module learning path

### Enrollments
- Unique constraint prevents duplicate enrollment in same module
- Target date is calculated from module estimated days
- Progress updates when items are marked complete
- Status changes: OnGoing → Paused or OnGoing → Completed

### Submissions
- Multiple submissions allowed per enrollment
- Only final approved submission counts as complete
- Manager can provide feedback on rejected submissions
- Allow re-submission after rejection

---

## API Usage Examples

### Example 1: Complete User Authentication Flow

```bash
# 1. Login
curl -X POST https://localhost:7118/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123"
  }'

# Response includes token
# Use token in subsequent requests

# 2. Get enrollments
curl -X GET https://localhost:7118/api/v1/enrollments \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..."
```

### Example 2: Module Enrollment and Progress Tracking

```bash
# 1. Get available modules
curl -X GET https://localhost:7118/api/v1/modules \
  -H "Authorization: Bearer {token}"

# 2. Create enrollment
curl -X POST https://localhost:7118/api/v1/enrollments \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"moduleId": "770e8400-e29b-41d4-a716-446655440000"}'

# 3. Get progress
curl -X GET https://localhost:7118/api/v1/enrollments/{id}/progress \
  -H "Authorization: Bearer {token}"

# 4. Mark item complete
curl -X POST https://localhost:7118/api/v1/enrollments/{id}/items \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "moduleItemId": "880e8400-e29b-41d4-a716-446655440000",
    "evidenceUrl": "https://github.com/user/project"
  }'
```

---

**Last Updated**: December 12, 2025  
**API Version**: v1  
**Development Base URL**: `https://localhost:7118`  
**Production Base URL**: `https://api.levelup.local`  
**Endpoint Format**: `{base_url}/api/v1/{resource}`
