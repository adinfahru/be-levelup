namespace LevelUp.API.DTOs.Dashboards;

public record EmployeeListResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    bool IsIdle
);