using LevelUp.API.DTOs.Auth;
using LevelUp.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LevelUp.API.Utilities;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest dto)
  {
    var result = await _authService.LoginAsync(dto);

    if (result == null)
      return Unauthorized(
          new ApiResponse<object>(
              StatusCodes.Status401Unauthorized,
              "Invalid email or password"
          )
      );

    return Ok(
        new ApiResponse<LoginResponse>(StatusCodes.Status200OK, "Login successful", result)
    );
  }

  [Authorize]
  [HttpPost("logout")]
  public IActionResult Logout()
  {
    // JWT is stateless, logout handled client-side by removing token
    return Ok(new { success = true, message = "Logged out successfully" });
  }

  [HttpPost("password/request")]
  public async Task<IActionResult> RequestPassword([FromBody] PasswordRequest dto)
  {
    await _authService.RequestPasswordChangeAsync(dto);
    // Always return 200 to avoid leaking account existence
    return Ok(new { success = true, message = "If the email exists, an OTP has been sent." });
  }

  [HttpPost("password/confirm")]
  public async Task<IActionResult> ConfirmPassword([FromBody] PasswordConfirmRequest dto)
  {
    await _authService.ConfirmPasswordChangeAsync(dto);
    return Ok(new { success = true, message = "Password changed successfully" });
  }
}
