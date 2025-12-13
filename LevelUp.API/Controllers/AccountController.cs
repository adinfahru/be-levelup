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
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await _userService.GetAllAccountsAsync(cancellationToken);
        return Ok(new ApiResponse<IEnumerable<UserResponse>>(200, "Success", data));

    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] UserRequest request, CancellationToken cancellationToken)
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
}
