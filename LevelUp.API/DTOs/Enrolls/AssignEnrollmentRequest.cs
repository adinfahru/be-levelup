namespace LevelUp.API.DTOs.Enrolls;

public record AssignEnrollmentRequest
(
    Guid EmployeeId,
    Guid ModuleId
);