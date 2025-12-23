using LevelUp.API.DTOs.Auth;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LevelUp.API.Utilities;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
  private readonly IAuthService _authService;
  private readonly LevelUpDbContext _db;

  public AuthController(IAuthService authService, LevelUpDbContext db)
  {
    _authService = authService;
    _db = db;
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

  [Authorize]
  [HttpGet("profile")]
  public async Task<IActionResult> GetProfile(CancellationToken cancellationToken = default)
  {
    var accountId = User.GetAccountId();

    var employee = await _db.Employees
        .Where(e => e.AccountId == accountId)
        .Select(e => new
        {
          e.Id,
          e.AccountId,
          e.FirstName,
          e.LastName,
          e.PositionId,
          e.IsIdle,
          e.CreatedAt,
          e.UpdatedAt
        })
        .FirstOrDefaultAsync(cancellationToken);

    if (employee == null)
      return NotFound(new ApiResponse<object>(StatusCodes.Status404NotFound, "Employee not found"));

    var account = await _db.Accounts
        .Where(a => a.Id == accountId)
        .Select(a => new { a.Id, a.Email, a.Role, a.IsActive })
        .FirstOrDefaultAsync(cancellationToken);

    var data = new { Account = account, Employee = employee };

    return Ok(new ApiResponse<object>(StatusCodes.Status200OK, "Success", data));
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
