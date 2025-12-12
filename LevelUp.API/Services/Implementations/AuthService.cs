using LevelUp.API.DTOs.Auth;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;

namespace LevelUp.API.Services.Implementations;

public class AuthService : IAuthService
{
  private readonly IAccountRepository _accountRepository;
  private readonly IHashHandler _hashHandler;
  private readonly IJwtTokenHandler _jwtTokenHandler;
  private readonly IConfiguration _configuration;

  public AuthService(
      IAccountRepository accountRepository,
      IHashHandler hashHandler,
      IJwtTokenHandler jwtTokenHandler,
      IConfiguration configuration)
  {
    _accountRepository = accountRepository;
    _hashHandler = hashHandler;
    _jwtTokenHandler = jwtTokenHandler;
    _configuration = configuration;
  }

  public async Task<LoginResponse?> LoginAsync(LoginRequest request)
  {
    var account = await _accountRepository.FirstOrDefaultAsync(x => x.Email == request.Email);

    if (account == null || !account.IsActive || string.IsNullOrEmpty(account.PasswordHash) || string.IsNullOrEmpty(account.Email))
      return null;

    if (!_hashHandler.ValidateHash(request.Password, account.PasswordHash))
      return null;

    var token = _jwtTokenHandler.GenerateToken(account.Email, account.Role.ToString());
    var expiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));

    return new LoginResponse(token, account.Email, account.Role.ToString(), expiresAt);
  }
}
