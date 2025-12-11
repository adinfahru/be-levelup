using LevelUp.API.Entity;

namespace LevelUp.API.DTOs.AccountManagement;

public record AccountResponse
(
    Guid AccountId,
    string Email,
    UserRole Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
