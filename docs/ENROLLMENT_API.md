# Enrollment API Documentation

## Table of Contents
1. [Overview](#overview)
2. [Enrollment Lifecycle](#enrollment-lifecycle)
3. [API Endpoints](#api-endpoints)
4. [Data Models](#data-models)
5. [Business Rules](#business-rules)
6. [Test Scenarios](#test-scenarios)
7. [Error Codes](#error-codes)

---

## Overview

The Enrollment API provides comprehensive functionality for employees to manage their learning journey through training modules. Employees can enroll in modules, track their progress, submit checklist items, and manage their enrollment status.

**Key Features:**
- Single active enrollment policy (one module at a time)
- Progress tracking with percentage completion
- Checklist item submission with evidence URLs
- Enrollment pause and resume capability
- Historical enrollment tracking
- Role-based access control (Employee only)

**Base URL:**
```
Development: http://localhost:5067/api/v1
Production: https://api.levelup.local/api/v1
```

---

## Enrollment Lifecycle

### Status Flow Diagram

```
Start
  ↓
[Employee Enrolls] → OnGoing
  ↓
[Employee Completes Items] → OnGoing (increasing progress)
  ↓
[All Items Completed] → Completed
  │
  └→ [Manual Pause] → Paused
       ↓
     [Resume] → OnGoing
```

### Enrollment Statuses

| Status | Description | Can Resume? | Can Submit Items? |
|--------|-------------|-------------|-------------------|
| `OnGoing` | Active enrollment, employee is working on module | N/A | Yes |
| `Paused` | Temporarily suspended by employee | Yes | No |
| `Completed` | All items finished, enrollment closed | No | No |

### Key Behaviors

1. **One Active Enrollment Rule**: Employees can only have ONE active (OnGoing) enrollment at a time
2. **Auto-Pause**: When an employee starts a new enrollment, any existing OnGoing enrollment is automatically paused
3. **Progress Calculation**: `currentProgress = (completedItems / totalItems) * 100`
4. **Auto-Complete**: When all checklist items are submitted, enrollment status automatically changes to Completed
5. **Resume Restriction**: Cannot resume if another enrollment is already OnGoing

---

## API Endpoints

### 1. Get Current Enrollment

Retrieve the employee's currently active (OnGoing) enrollment.

**Endpoint:**
```
GET /api/v1/enrollments/current
```

**Authorization:** Bearer Token (Employee role required)

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "enrollmentId": "60000000-0000-0000-0000-000000000001",
    "moduleId": "40000000-0000-0000-0000-000000000001",
    "moduleTitle": "ASP.NET Core Fundamentals",
    "moduleDescription": "Learn the basics of building web APIs",
    "startDate": "2024-01-15T08:00:00Z",
    "targetDate": "2024-02-15T08:00:00Z",
    "completedDate": null,
    "status": "OnGoing",
    "currentProgress": 33,
    "sections": [
      {
        "enrollmentItemId": "70000000-0000-0000-0000-000000000001",
        "moduleItemId": "50000000-0000-0000-0000-000000000001",
        "orderIndex": 1,
        "moduleItemTitle": "Setup Development Environment",
        "moduleItemDescription": "Install VS Code, .NET SDK, and configure workspace",
        "moduleItemUrl": "https://learn.microsoft.com/dotnet/core/install/",
        "isFinalSubmission": false,
        "isCompleted": true,
        "evidenceUrl": "https://github.com/johndoe/setup-proof",
        "completedAt": "2024-01-16T10:30:00Z"
      },
      {
        "enrollmentItemId": "70000000-0000-0000-0000-000000000002",
        "moduleItemId": "50000000-0000-0000-0000-000000000002",
        "orderIndex": 2,
        "moduleItemTitle": "Create First Web API",
        "moduleItemDescription": "Build a simple REST API with CRUD operations",
        "moduleItemUrl": "https://learn.microsoft.com/aspnet/core/tutorials/first-web-api",
        "isFinalSubmission": false,
        "isCompleted": false,
        "evidenceUrl": null,
        "completedAt": null
      },
      {
        "enrollmentItemId": "70000000-0000-0000-0000-000000000003",
        "moduleItemId": "50000000-0000-0000-0000-000000000003",
        "orderIndex": 3,
        "moduleItemTitle": "Final Project Submission",
        "moduleItemDescription": "Complete project with authentication and database",
        "moduleItemUrl": "https://github.com/company/final-project-template",
        "isFinalSubmission": true,
        "isCompleted": false,
        "evidenceUrl": null,
        "completedAt": null
      }
    ]
  }
}
```

**Response (204 No Content):**
When employee has no active enrollment (idle or all enrollments completed/paused).

**Error Responses:**

| Status | Error Code | Description |
|--------|-----------|-------------|
| 401 | UNAUTHORIZED | Missing or invalid Bearer token |
| 403 | FORBIDDEN | Non-employee role attempting access |

---

### 2. Get Enrollment History

Retrieve all past enrollments (Completed and Paused status).

**Endpoint:**
```
GET /api/v1/enrollments/history
```

**Authorization:** Bearer Token (Employee role required)

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "enrollmentId": "60000000-0000-0000-0000-000000000002",
      "moduleId": "40000000-0000-0000-0000-000000000002",
      "moduleTitle": "Advanced C# Programming",
      "moduleDescription": "Master async/await, LINQ, delegates, and generics",
      "startDate": "2023-11-01T08:00:00Z",
      "targetDate": "2023-12-15T08:00:00Z",
      "completedDate": "2023-12-10T14:20:00Z",
      "status": "Completed",
      "currentProgress": 100,
      "sections": [
        {
          "enrollmentItemId": "70000000-0000-0000-0000-000000000010",
          "moduleItemId": "50000000-0000-0000-0000-000000000004",
          "orderIndex": 1,
          "moduleItemTitle": "Async/Await Patterns",
          "moduleItemDescription": "Learn asynchronous programming in C#",
          "moduleItemUrl": "https://learn.microsoft.com/dotnet/csharp/async",
          "isFinalSubmission": false,
          "isCompleted": true,
          "evidenceUrl": "https://github.com/jessica/async-project",
          "completedAt": "2023-11-15T09:00:00Z"
        }
        // ... more items
      ]
    },
    {
      "enrollmentId": "60000000-0000-0000-0000-000000000004",
      "moduleId": "40000000-0000-0000-0000-000000000005",
      "moduleTitle": "React Fundamentals",
      "moduleDescription": "Build modern web applications with React",
      "startDate": "2024-01-01T08:00:00Z",
      "targetDate": "2024-02-01T08:00:00Z",
      "completedDate": null,
      "status": "Paused",
      "currentProgress": 50,
      "sections": [
        // ... checklist items
      ]
    }
  ]
}
```

**Response (200 OK - Empty History):**
```json
{
  "success": true,
  "data": []
}
```

**Error Responses:**

| Status | Error Code | Description |
|--------|-----------|-------------|
| 401 | UNAUTHORIZED | Missing or invalid Bearer token |
| 403 | FORBIDDEN | Non-employee role attempting access |

---

### 3. Create Enrollment

Enroll employee in a new training module.

**Endpoint:**
```
POST /api/v1/enrollments
```

**Authorization:** Bearer Token (Employee role required)

**Request Body:**
```json
{
  "moduleId": "40000000-0000-0000-0000-000000000001"
}
```

**Field Validation:**
- `moduleId` (required): Must be a valid, active module GUID

**Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "enrollmentId": "60000000-0000-0000-0000-000000000008",
    "moduleId": "40000000-0000-0000-0000-000000000001",
    "moduleTitle": "ASP.NET Core Fundamentals",
    "moduleDescription": "Learn the basics of building web APIs",
    "startDate": "2024-12-16T10:30:00Z",
    "targetDate": "2025-01-16T10:30:00Z",
    "completedDate": null,
    "status": "OnGoing",
    "currentProgress": 0,
    "sections": [
      {
        "enrollmentItemId": "70000000-0000-0000-0000-000000000025",
        "moduleItemId": "50000000-0000-0000-0000-000000000001",
        "orderIndex": 1,
        "moduleItemTitle": "Setup Development Environment",
        "moduleItemDescription": "Install VS Code, .NET SDK, and configure workspace",
        "moduleItemUrl": "https://learn.microsoft.com/dotnet/core/install/",
        "isFinalSubmission": false,
        "isCompleted": false,
        "evidenceUrl": null,
        "completedAt": null
      }
      // ... all module items initialized
    ]
  },
  "message": "Successfully enrolled in module"
}
```

**Business Logic:**
1. If employee has an existing OnGoing enrollment → automatically paused
2. Target date = Start date + Module's `estimatedDays`
3. All module items are automatically created as EnrollmentItems with `isCompleted = false`

**Error Responses:**

| Status | Error Code | Message | Scenario |
|--------|-----------|---------|----------|
| 400 | ENROLLMENT_ALREADY_EXISTS | Employee already has an active enrollment | Employee tries to enroll while having OnGoing status |
| 400 | CANNOT_ENROLL_INACTIVE_MODULE | Module is not active | Module's `isActive = false` |
| 404 | MODULE_NOT_FOUND | Module does not exist | Invalid module GUID |
| 401 | UNAUTHORIZED | Authentication required | Missing Bearer token |
| 403 | FORBIDDEN | Employee role required | Non-employee trying to enroll |

---

### 4. Submit Checklist Item

Mark a module item as completed with optional evidence and feedback.

**Endpoint:**
```
POST /api/v1/enrollments/{enrollmentId}/items
```

**Path Parameters:**
- `enrollmentId` (required): GUID of the enrollment

**Authorization:** Bearer Token (Employee role required)

**Request Body:**
```json
{
  "moduleItemId": "50000000-0000-0000-0000-000000000002",
  "evidenceUrl": "https://github.com/johndoe/first-api-project",
  "feedback": "Successfully created REST API with authentication"
}
```

**Field Validation:**
- `moduleItemId` (required): Must be valid module item GUID belonging to the enrollment's module
- `evidenceUrl` (optional): Valid URL format, max 500 characters
- `feedback` (optional): Max 1000 characters

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "enrollmentId": "60000000-0000-0000-0000-000000000001",
    "moduleId": "40000000-0000-0000-0000-000000000001",
    "moduleTitle": "ASP.NET Core Fundamentals",
    "moduleDescription": "Learn the basics of building web APIs",
    "startDate": "2024-01-15T08:00:00Z",
    "targetDate": "2024-02-15T08:00:00Z",
    "completedDate": null,
    "status": "OnGoing",
    "currentProgress": 66,
    "sections": [
      {
        "enrollmentItemId": "70000000-0000-0000-0000-000000000002",
        "moduleItemId": "50000000-0000-0000-0000-000000000002",
        "orderIndex": 2,
        "moduleItemTitle": "Create First Web API",
        "moduleItemDescription": "Build a simple REST API with CRUD operations",
        "moduleItemUrl": "https://learn.microsoft.com/aspnet/core/tutorials/first-web-api",
        "isFinalSubmission": false,
        "isCompleted": true,
        "evidenceUrl": "https://github.com/johndoe/first-api-project",
        "completedAt": "2024-12-16T14:25:00Z"
      }
      // ... other items
    ]
  },
  "message": "Checklist item submitted successfully"
}
```

**Business Logic:**
1. Item is marked as completed (`isCompleted = true`)
2. `completedAt` timestamp is set to current UTC time
3. Enrollment's `currentProgress` is recalculated
4. If all items completed → enrollment status changes to `Completed` and `completedDate` is set

**Error Responses:**

| Status | Error Code | Message | Scenario |
|--------|-----------|---------|----------|
| 400 | ITEM_ALREADY_COMPLETED | Checklist item already marked as completed | Re-submitting completed item |
| 400 | ITEM_NOT_IN_MODULE | Module item does not belong to this enrollment's module | Submitting item from different module |
| 403 | FORBIDDEN | Cannot submit items for another employee's enrollment | Wrong employee accessing enrollment |
| 404 | ENROLLMENT_NOT_FOUND | Enrollment does not exist | Invalid enrollment GUID |
| 404 | MODULE_ITEM_NOT_FOUND | Module item does not exist | Invalid module item GUID |

---

### 5. Get Enrollment Progress

Retrieve detailed progress information for a specific enrollment.

**Endpoint:**
```
GET /api/v1/enrollments/{enrollmentId}/progress
```

**Path Parameters:**
- `enrollmentId` (required): GUID of the enrollment

**Authorization:** Bearer Token (Employee role required)

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "enrollmentId": "60000000-0000-0000-0000-000000000001",
    "moduleId": "40000000-0000-0000-0000-000000000001",
    "moduleTitle": "ASP.NET Core Fundamentals",
    "moduleDescription": "Learn the basics of building web APIs",
    "startDate": "2024-01-15T08:00:00Z",
    "targetDate": "2024-02-15T08:00:00Z",
    "completedDate": null,
    "status": "OnGoing",
    "currentProgress": 66,
    "sections": [
      {
        "enrollmentItemId": "70000000-0000-0000-0000-000000000001",
        "moduleItemId": "50000000-0000-0000-0000-000000000001",
        "orderIndex": 1,
        "moduleItemTitle": "Setup Development Environment",
        "moduleItemDescription": "Install VS Code, .NET SDK, and configure workspace",
        "moduleItemUrl": "https://learn.microsoft.com/dotnet/core/install/",
        "isFinalSubmission": false,
        "isCompleted": true,
        "evidenceUrl": "https://github.com/johndoe/setup-proof",
        "completedAt": "2024-01-16T10:30:00Z"
      },
      {
        "enrollmentItemId": "70000000-0000-0000-0000-000000000002",
        "moduleItemId": "50000000-0000-0000-0000-000000000002",
        "orderIndex": 2,
        "moduleItemTitle": "Create First Web API",
        "moduleItemDescription": "Build a simple REST API with CRUD operations",
        "moduleItemUrl": "https://learn.microsoft.com/aspnet/core/tutorials/first-web-api",
        "isFinalSubmission": false,
        "isCompleted": true,
        "evidenceUrl": "https://github.com/johndoe/first-api-project",
        "completedAt": "2024-12-16T14:25:00Z"
      },
      {
        "enrollmentItemId": "70000000-0000-0000-0000-000000000003",
        "moduleItemId": "50000000-0000-0000-0000-000000000003",
        "orderIndex": 3,
        "moduleItemTitle": "Final Project Submission",
        "moduleItemDescription": "Complete project with authentication and database",
        "moduleItemUrl": "https://github.com/company/final-project-template",
        "isFinalSubmission": true,
        "isCompleted": false,
        "evidenceUrl": null,
        "completedAt": null
      }
    ]
  }
}
```

**Progress Metrics:**
- `currentProgress`: Percentage (0-100) = (completed items / total items) × 100
- `sections`: All checklist items with completion status and timestamps
- `isFinalSubmission`: Indicates if item is the final project/submission

**Error Responses:**

| Status | Error Code | Message | Scenario |
|--------|-----------|---------|----------|
| 403 | FORBIDDEN | Cannot view another employee's progress | Wrong employee accessing enrollment |
| 404 | ENROLLMENT_NOT_FOUND | Enrollment does not exist | Invalid enrollment GUID |
| 401 | UNAUTHORIZED | Authentication required | Missing Bearer token |

---

### 6. Resume Enrollment

Resume a paused enrollment to continue working on the module.

**Endpoint:**
```
POST /api/v1/enrollments/{enrollmentId}/resume
```

**Path Parameters:**
- `enrollmentId` (required): GUID of the paused enrollment

**Authorization:** Bearer Token (Employee role required)

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "enrollmentId": "60000000-0000-0000-0000-000000000004",
    "moduleId": "40000000-0000-0000-0000-000000000005",
    "moduleTitle": "React Fundamentals",
    "moduleDescription": "Build modern web applications with React",
    "startDate": "2024-01-01T08:00:00Z",
    "targetDate": "2024-02-01T08:00:00Z",
    "completedDate": null,
    "status": "OnGoing",
    "currentProgress": 50,
    "sections": [
      // ... checklist items
    ]
  },
  "message": "Enrollment resumed successfully"
}
```

**Business Logic:**
1. Enrollment status changes from `Paused` to `OnGoing`
2. Employee can now submit checklist items for this enrollment
3. If employee has another OnGoing enrollment → that enrollment is automatically paused

**Error Responses:**

| Status | Error Code | Message | Scenario |
|--------|-----------|---------|----------|
| 400 | ENROLLMENT_NOT_PAUSED | Enrollment is not in paused status | Trying to resume OnGoing or Completed enrollment |
| 400 | ALREADY_HAS_ACTIVE_ENROLLMENT | Employee already has an active enrollment | Another enrollment is OnGoing (edge case) |
| 400 | CANNOT_RESUME_COMPLETED | Cannot resume completed enrollment | Trying to resume Completed status |
| 403 | FORBIDDEN | Cannot resume another employee's enrollment | Wrong employee accessing enrollment |
| 404 | ENROLLMENT_NOT_FOUND | Enrollment does not exist | Invalid enrollment GUID |

---

## Data Models

### EnrollmentRequest

```typescript
{
  moduleId: string (GUID, required)
}
```

### SubmitChecklistRequest

```typescript
{
  moduleItemId: string (GUID, required),
  evidenceUrl?: string (optional, max 500 chars, URL format),
  feedback?: string (optional, max 1000 chars)
}
```

### EnrollmentResponse

```typescript
{
  enrollmentId: string (GUID),
  moduleId: string (GUID),
  moduleTitle: string,
  moduleDescription: string,
  startDate: string (ISO 8601),
  targetDate: string (ISO 8601),
  completedDate: string | null (ISO 8601),
  status: "OnGoing" | "Paused" | "Completed",
  currentProgress: number (0-100),
  sections: EnrollmentItemDto[]
}
```

### EnrollmentItemDto

```typescript
{
  enrollmentItemId: string (GUID),
  moduleItemId: string (GUID),
  orderIndex: number,
  moduleItemTitle: string,
  moduleItemDescription: string,
  moduleItemUrl: string,
  isFinalSubmission: boolean,
  isCompleted: boolean,
  evidenceUrl: string | null,
  completedAt: string | null (ISO 8601)
}
```

---

## Business Rules

### 1. Single Active Enrollment Policy

**Rule:** An employee can only have ONE active (OnGoing) enrollment at any time.

**Enforcement:**
- When creating new enrollment → existing OnGoing enrollment is automatically paused
- When resuming enrollment → any other OnGoing enrollment is automatically paused
- Employee cannot enroll in multiple modules simultaneously

**Example:**
```
Employee has: Enrollment A (OnGoing, Module 1)
Employee enrolls in Module 2:
  → Enrollment A status: OnGoing → Paused
  → Enrollment B status: OnGoing (new)
```

### 2. Progress Calculation

**Formula:**
```
currentProgress = (completedItems / totalItems) × 100
```

**Rules:**
- Rounded to nearest integer
- Minimum: 0% (no items completed)
- Maximum: 100% (all items completed)
- Recalculated after each item submission

**Example:**
```
Module has 5 items:
- 0 completed → 0%
- 1 completed → 20%
- 2 completed → 40%
- 3 completed → 60%
- 4 completed → 80%
- 5 completed → 100% → Auto-complete enrollment
```

### 3. Auto-Completion

**Trigger:** When employee submits the last remaining incomplete item.

**Actions:**
1. Enrollment status: OnGoing → Completed
2. `completedDate` field is set to current UTC timestamp
3. `currentProgress` becomes 100%
4. Employee can no longer submit items for this enrollment
5. Employee becomes idle (can enroll in new module)

### 4. Module Activity Check

**Rule:** Employees can only enroll in active modules (`isActive = true`).

**Validation:**
- Checked during enrollment creation
- Returns 400 error if module is inactive
- Existing enrollments are NOT affected when module becomes inactive

**Example:**
```
Module 3 (Microservices): isActive = false
Employee tries to enroll → 400 Bad Request
Employee already enrolled in Module 3 → Can continue, unaffected
```

### 5. Checklist Item Validation

**Rules:**
- Item must belong to the enrollment's module
- Item cannot be submitted twice (no re-submission)
- Only employee who owns the enrollment can submit
- Evidence URL must be valid format if provided

**Validation Flow:**
```
1. Check enrollment exists → 404 if not
2. Check employee owns enrollment → 403 if not
3. Check item exists → 404 if not
4. Check item belongs to module → 400 if not
5. Check item not already completed → 400 if completed
6. Validate evidence URL format → 400 if invalid
7. Mark item as completed ✓
```

### 6. Resume Restrictions

**Rules:**
- Can only resume enrollments with status = Paused
- Cannot resume Completed enrollments
- Cannot resume OnGoing enrollments (no-op)
- If another enrollment is OnGoing → automatically paused

**State Transitions:**
```
Paused → Resume → OnGoing ✓
OnGoing → Resume → 400 Error ✗
Completed → Resume → 400 Error ✗
```

---

## Test Scenarios

### Scenario 1: New Employee First Enrollment

**Given:** Employee has never enrolled in any module
**When:** Employee enrolls in Module 1
**Then:**
- ✓ Enrollment created with status = OnGoing
- ✓ Progress = 0%
- ✓ All module items initialized as uncompleted
- ✓ Target date = current date + module's estimatedDays

**Test Case:**
```http
POST /api/v1/enrollments
Authorization: Bearer {employee6Token}

{
  "moduleId": "40000000-0000-0000-0000-000000000001"
}
```

---

### Scenario 2: Employee with Active Enrollment Enrolls Again

**Given:** Employee has OnGoing enrollment in Module 1
**When:** Employee tries to enroll in Module 2
**Then:**
- ✓ Previous enrollment (Module 1) auto-paused
- ✓ New enrollment (Module 2) created with OnGoing status
- ✓ Employee can now work on Module 2

**Test Cases:**
```http
# First enrollment
POST /api/v1/enrollments
{
  "moduleId": "40000000-0000-0000-0000-000000000001"
}

# Second enrollment (auto-pauses first)
POST /api/v1/enrollments
{
  "moduleId": "40000000-0000-0000-0000-000000000002"
}

# Verify first is paused
GET /api/v1/enrollments/history
# Should show Module 1 with status = Paused
```

---

### Scenario 3: Progress Tracking

**Given:** Employee enrolled in Module 1 (3 items)
**When:** Employee submits items sequentially
**Then:**
- Item 1 completed → Progress = 33%
- Item 2 completed → Progress = 66%
- Item 3 completed → Progress = 100%, Status = Completed

**Test Cases:**
```http
# Submit Item 1
POST /api/v1/enrollments/{enrollmentId}/items
{
  "moduleItemId": "50000000-0000-0000-0000-000000000001",
  "evidenceUrl": "https://github.com/user/item1"
}
# Response: currentProgress = 33

# Submit Item 2
POST /api/v1/enrollments/{enrollmentId}/items
{
  "moduleItemId": "50000000-0000-0000-0000-000000000002",
  "evidenceUrl": "https://github.com/user/item2"
}
# Response: currentProgress = 66

# Submit Item 3 (final)
POST /api/v1/enrollments/{enrollmentId}/items
{
  "moduleItemId": "50000000-0000-0000-0000-000000000003",
  "evidenceUrl": "https://github.com/user/item3"
}
# Response: currentProgress = 100, status = "Completed"
```

---

### Scenario 4: Pause and Resume

**Given:** Employee has OnGoing enrollment, needs to take break
**When:** Employee enrolls in another module (auto-pause) then resumes old one
**Then:**
- ✓ Old enrollment paused
- ✓ New enrollment becomes active
- ✓ Can resume old enrollment later
- ✓ Resuming auto-pauses the newer enrollment

**Test Cases:**
```http
# Current: Module 1 (OnGoing)
# Enroll in Module 2 → Module 1 auto-paused
POST /api/v1/enrollments
{
  "moduleId": "40000000-0000-0000-0000-000000000002"
}

# Resume Module 1 → Module 2 auto-paused
POST /api/v1/enrollments/{module1EnrollmentId}/resume

# Check history
GET /api/v1/enrollments/history
# Module 2 should now be Paused

# Check current
GET /api/v1/enrollments/current
# Module 1 should be OnGoing
```

---

### Scenario 5: Invalid Submissions

**Test Submitting Wrong Employee:**
```http
# Employee 1 trying to submit for Employee 2's enrollment
POST /api/v1/enrollments/{employee2EnrollmentId}/items
Authorization: Bearer {employee1Token}

{
  "moduleItemId": "..."
}
# Expected: 403 Forbidden
```

**Test Submitting Completed Item:**
```http
# Submit item that was already completed
POST /api/v1/enrollments/{enrollmentId}/items
{
  "moduleItemId": "50000000-0000-0000-0000-000000000001"
}
# Expected: 400 Bad Request - Item already completed
```

**Test Submitting Item from Different Module:**
```http
# Enrolled in Module 1, trying to submit Module 2 item
POST /api/v1/enrollments/{module1EnrollmentId}/items
{
  "moduleItemId": "50000000-0000-0000-0000-000000000004"
}
# Expected: 400 Bad Request - Item not in module
```

---

### Scenario 6: Idle Employee History

**Given:** Employee completed Module 1, now idle (no active enrollment)
**When:** Request current enrollment and history
**Then:**
- ✓ Current enrollment → 204 No Content
- ✓ History → Contains completed Module 1

**Test Cases:**
```http
# Get current (should be empty)
GET /api/v1/enrollments/current
# Expected: 204 No Content

# Get history (should show completed)
GET /api/v1/enrollments/history
# Expected: 200 OK with completed enrollment
```

---

## Error Codes

### Enrollment Errors

| Code | HTTP Status | Message | Resolution |
|------|-------------|---------|------------|
| ENROLLMENT_NOT_FOUND | 404 | Enrollment does not exist | Verify enrollment ID is correct |
| ENROLLMENT_ALREADY_EXISTS | 400 | Employee already has an active enrollment | Complete or pause current enrollment first |
| ENROLLMENT_NOT_PAUSED | 400 | Enrollment is not in paused status | Only paused enrollments can be resumed |
| CANNOT_RESUME_COMPLETED | 400 | Cannot resume completed enrollment | Enroll in a new module instead |
| ALREADY_HAS_ACTIVE_ENROLLMENT | 400 | Employee already has an active enrollment | System should auto-pause, contact support |

### Module Errors

| Code | HTTP Status | Message | Resolution |
|------|-------------|---------|------------|
| MODULE_NOT_FOUND | 404 | Module does not exist | Verify module ID is correct |
| CANNOT_ENROLL_INACTIVE_MODULE | 400 | Module is not active | Choose an active module |
| MODULE_ITEM_NOT_FOUND | 404 | Module item does not exist | Verify item ID is correct |
| ITEM_NOT_IN_MODULE | 400 | Module item does not belong to this enrollment's module | Submit items from correct module |

### Checklist Errors

| Code | HTTP Status | Message | Resolution |
|------|-------------|---------|------------|
| ITEM_ALREADY_COMPLETED | 400 | Checklist item already marked as completed | Cannot resubmit completed items |
| INVALID_EVIDENCE_URL | 400 | Evidence URL format is invalid | Provide valid URL (http/https) |

### Authorization Errors

| Code | HTTP Status | Message | Resolution |
|------|-------------|---------|------------|
| UNAUTHORIZED | 401 | Authentication required | Provide valid Bearer token |
| FORBIDDEN | 403 | Employee role required | Only employees can access enrollment APIs |
| FORBIDDEN | 403 | Cannot access another employee's enrollment | Only owner can view/modify enrollment |

---

## Implementation Notes

### Performance Considerations

1. **Eager Loading**: Enrollment responses include all checklist items (sections) in a single query
2. **Progress Calculation**: Computed at submission time, not on every read
3. **History Queries**: Filtered by status (Paused, Completed) to exclude OnGoing

### Database Indexes

```sql
-- Recommended indexes for performance
CREATE INDEX idx_enrollment_employee_status ON Enrollments (EmployeeId, Status);
CREATE INDEX idx_enrollment_status ON Enrollments (Status);
CREATE INDEX idx_enrollmentitem_enrollment ON EnrollmentItems (EnrollmentId);
CREATE INDEX idx_enrollmentitem_completed ON EnrollmentItems (IsCompleted);
```

### JWT Claims Required

```json
{
  "sub": "20000000-0000-0000-0000-000000000003",
  "role": "Employee",
  "email": "employee@levelup.com",
  "employeeId": "30000000-0000-0000-0000-000000000003"
}
```

**Key Claims:**
- `sub`: Account ID (used for authorization)
- `role`: Must be "Employee"
- `employeeId`: Employee entity ID (used for data queries)

---

## Testing with Fixed GUIDs

All enrollment test data uses fixed GUIDs for consistency:

**Enrollments:**
- `60000000-0000-0000-0000-000000000001` → John, Module 1 (OnGoing, 33%)
- `60000000-0000-0000-0000-000000000002` → Jessica, Module 2 (Completed)
- `60000000-0000-0000-0000-000000000003` → Jessica, Module 4 (OnGoing, 66%)
- `60000000-0000-0000-0000-000000000004` → Christopher, Module 5 (Paused, 50%)
- `60000000-0000-0000-0000-000000000005` → Christopher, Module 1 (Completed)
- `60000000-0000-0000-0000-000000000006` → Daniel, Module 2 (OnGoing, 40%)
- `60000000-0000-0000-0000-000000000007` → Amanda, Module 5 (Completed)

**Test Employees:**
- Employee 3 (John): Has ongoing enrollment
- Employee 4 (Jessica): Has completed + ongoing
- Employee 5 (Christopher): Has paused + completed
- Employee 6 (Amanda): Has completed only (idle)
- Employee 8 (Michael): Never enrolled (idle)

**Complete test suite:** See `api-tests.http` section 6 (30 comprehensive test cases)

---

## Changelog

| Date | Version | Changes |
|------|---------|---------|
| 2024-12-16 | 1.0.0 | Initial enrollment API documentation |

---

## Support

For API issues or questions:
- Email: support@levelup.local
- Slack: #levelup-api-support
- Documentation: [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)
