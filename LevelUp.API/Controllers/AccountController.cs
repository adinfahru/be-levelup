using LevelUp.API.DTOs.AccountManagement;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? role = null,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var (data, total) = await _userService.GetAllAccountsAsync(page, limit, role, search, isActive, cancellationToken);
        return Ok(new ApiResponse<IEnumerable<UserResponse>>(200, "Success", data, total));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var data = await _userService.GetAccountByIdAsync(id, cancellationToken);
        return Ok(new ApiResponse<UserResponse>(200, "Success", data));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(
        [FromBody] UserRequest request,
        CancellationToken cancellationToken = default)
    {
        await _userService.CreateAccountAsync(request, cancellationToken);
        return Ok(new ApiResponse<UserResponse>("Account Success Create"));
    }

    [HttpPut("{accountId}")]
    public async Task<IActionResult> UpdateAccount(Guid accountId, [FromBody] UserRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateAccountAsync(accountId, request, cancellationToken);
        return Ok(new ApiResponse<object>("Account Success Update"));
    }

    [HttpDelete("{accountId}")]
    public async Task<IActionResult> DeleteAccount(Guid accountId, CancellationToken cancellationToken)
    {
        await _userService.DeleteAccountAsync(accountId, cancellationToken);
        return Ok(new ApiResponse<object>("Account Success Delete"));
    }

    [HttpPut("{accountId}/activate")]
    public async Task<IActionResult> ActivateAccount(Guid accountId, CancellationToken cancellationToken)
    {
        await _userService.ActivateAccountAsync(accountId, cancellationToken);
        return Ok(new ApiResponse<object>("Account activated successfully"));
    }
}
