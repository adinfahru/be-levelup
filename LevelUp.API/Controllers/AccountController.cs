using LevelUp.API.DTOs.AccountManagement;
using LevelUp.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LevelUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("admin/users")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var accounts = await _accountService.GetAllAccountsAsync(cancellationToken);
        return Ok(accounts);
    }

    [HttpPost("admin/users")]
    public async Task<IActionResult> CreateAccount(AccountRequest request, CancellationToken cancellationToken)
    {
        await _accountService.CreateAccountAsync(request, cancellationToken);
        return Ok("Succes Create");
    }

    [HttpPut("admin/users/{accountId}")]
    public async Task<IActionResult> UpdateAccount(Guid accountId, AccountRequest request, CancellationToken cancellationToken)
    {
        await _accountService.UpdateAccountAsync(accountId, request, cancellationToken);
        return Ok("Succes Update");
    }

    [HttpDelete("admin/users/{accountId}")]
    public async Task<IActionResult> DeleteAccount(Guid accountId, CancellationToken cancellationToken)
    {
        await _accountService.DeleteAccountAsync(accountId, cancellationToken);
        return Ok("Succes Delete");
    }
}
