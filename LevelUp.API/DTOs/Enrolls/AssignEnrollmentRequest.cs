namespace LevelUp.API.DTOs.Enrolls;

public record AssignEnrollmentRequest
(
    Guid AccountId,
    Guid ModuleId
);