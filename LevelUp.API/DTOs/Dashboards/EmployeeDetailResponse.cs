namespace LevelUp.API.DTOs.Dashboards;

public record EmployeeDetailResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Status,
    IEnumerable<string> Modules
);
