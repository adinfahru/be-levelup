using LevelUp.API.DTOs.Auth;

namespace LevelUp.API.Services.Interfaces;

public interface IAuthService
{
  Task<LoginResponse?> LoginAsync(LoginRequest request);
  Task RequestPasswordChangeAsync(PasswordRequest request);
  Task ConfirmPasswordChangeAsync(PasswordConfirmRequest request);
}
