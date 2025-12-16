namespace LevelUp.API.DTOs.Dashboards;

public record EmployeeDetailResponse(
    Guid Id,
    string FirstName,
    string LastName,
    bool IsIdle,
    string Email,
    string Role,
    string? PositionName
);