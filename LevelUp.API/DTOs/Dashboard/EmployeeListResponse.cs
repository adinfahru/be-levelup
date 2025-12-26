namespace LevelUp.API.DTOs.Dashboards;

public record EmployeeListResponse(
    Guid Id,
    Guid AccountId,
    string FirstName,
    string LastName,
    string Email,
    bool IsIdle
);