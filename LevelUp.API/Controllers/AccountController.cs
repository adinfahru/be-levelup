using LevelUp.API.DTOs.AccountManagement;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("admin/users")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await _userService.GetAllAccountsAsync(cancellationToken);
        return Ok(new ApiResponse<IEnumerable<UserResponse>>(200,"Success", data));
        
    }

    [HttpPost("admin/users")]
    public async Task<IActionResult> CreateAccount(UserRequest request, CancellationToken cancellationToken)
    {
        await _userService.CreateAccountAsync(request, cancellationToken);
        return Ok(new ApiResponse<UserResponse>("Account Success Create"));
    }

    [HttpPut("admin/users/{accountId}")]
    public async Task<IActionResult> UpdateAccount(Guid accountId, UserRequest request, CancellationToken cancellationToken)
    {
        await _userService.UpdateAccountAsync(accountId, request, cancellationToken);
        return Ok(new ApiResponse<object>("Account Success Update"));
    }

    [HttpDelete("admin/users/{accountId}")]
    public async Task<IActionResult> DeleteAccount(Guid accountId, CancellationToken cancellationToken)
    {
        await _userService.DeleteAccountAsync(accountId, cancellationToken);
        return Ok(new ApiResponse<object>("Account Success Delete"));
    }
}
