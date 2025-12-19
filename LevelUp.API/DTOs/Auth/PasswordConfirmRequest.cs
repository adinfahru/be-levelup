namespace LevelUp.API.DTOs.Auth;

public record PasswordConfirmRequest(
    string Email,
    string Otp,
    string NewPassword
);
