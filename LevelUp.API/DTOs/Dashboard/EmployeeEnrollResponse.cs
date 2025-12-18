using LevelUp.API.Entity;

public record EmployeeEnrollResponse(
    Guid EmployeeId,
    string FirstName,
    string LastName,
    string Email,
    bool IsIdle,
    string EnrollmentStatus
);
