using LevelUp.API.Entity;

namespace LevelUp.API.DTOs.AccountManagement;

public record AccountRequest
(
    string Email,
    string Password,
    UserRole Role
);
