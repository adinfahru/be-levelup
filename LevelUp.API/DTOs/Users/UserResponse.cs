using LevelUp.API.Entity;

namespace LevelUp.API.DTOs.AccountManagement;

public record UserResponse
(
    Guid AccountId,
    string Email,
    UserRole Role,
    bool IsActive,
    string FirstName,
    string LastName,
    Guid? PositionId,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
