namespace LevelUp.API.DTOs.Submissions;

public record SubmissionDetailResponse(
    Guid SubmissionId,
    string Status,

    // Employee
    string EmployeeName,
    string EmployeeEmail,

    // Module
    string ModuleTitle,

    // Notes
    string? Notes,
    string? ManagerFeedback,

    // Progress
    int CompletedSection,
    int TotalSection,

    IEnumerable<SubmissionSectionResponse> Sections
);
