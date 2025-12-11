using LevelUp.API.Entity;

namespace LevelUp.API.DTOs.AccountManagement;

public record UserRequest
(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role,
    Guid? PositionId
);
