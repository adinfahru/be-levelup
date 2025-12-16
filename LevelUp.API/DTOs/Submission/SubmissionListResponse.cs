namespace LevelUp.API.DTOs.Submissions;

public record SubmissionListResponse(
    Guid SubmissionId,
    Guid EnrollmentId,

    // Employee
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeEmail,

    // Module
    Guid ModuleId,
    string ModuleTitle,

    // Progress
    int CompletedSection,
    int TotalSection,

    // Submission
    string Status,
    DateTime SubmittedAt
);
