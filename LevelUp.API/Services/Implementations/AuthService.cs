using LevelUp.API.DTOs.Auth;
using LevelUp.API.Repositories.Interfaces;
using LevelUp.API.Services.Interfaces;
using LevelUp.API.Utilities;
using MentorHub.API.Utilities;

namespace LevelUp.API.Services.Implementations;

public class AuthService : IAuthService
{
  private readonly IAccountRepository _accountRepository;
  private readonly IHashHandler _hashHandler;
  private readonly IJwtTokenHandler _jwtTokenHandler;
  private readonly IConfiguration _configuration;
  private readonly IEmailHandler _emailHandler;

  public AuthService(
      IAccountRepository accountRepository,
      IHashHandler hashHandler,
      IJwtTokenHandler jwtTokenHandler,
      IConfiguration configuration,
      IEmailHandler emailHandler)
  {
    _accountRepository = accountRepository;
    _hashHandler = hashHandler;
    _jwtTokenHandler = jwtTokenHandler;
    _configuration = configuration;
    _emailHandler = emailHandler;
  }

  public async Task<LoginResponse?> LoginAsync(LoginRequest request)
  {
    var account = await _accountRepository.FirstOrDefaultAsync(x => x.Email == request.Email);

    if (account == null || !account.IsActive || string.IsNullOrEmpty(account.PasswordHash) || string.IsNullOrEmpty(account.Email))
      return null;

    if (!_hashHandler.ValidateHash(request.Password, account.PasswordHash))
      return null;

    var token = _jwtTokenHandler.GenerateToken(account.Id.ToString(), account.Email, account.Role.ToString());
    var expiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));

    return new LoginResponse(token, account.Email, account.Role.ToString(), expiresAt);
  }

  public async Task RequestPasswordChangeAsync(PasswordRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Email))
      return;

    var account = await _accountRepository.GetByEmailAsync(request.Email, CancellationToken.None);

    // Always return success to caller (do not reveal existence)
    if (account == null)
      return;

    // generate 6-digit OTP
    var rng = new Random();
    var otp = rng.Next(100000, 999999).ToString();

    // hash OTP and save
    account.OtpHash = _hashHandler.HashPassword(otp);
    account.OtpExpiresAt = DateTime.UtcNow.AddMinutes(15);
    account.OtpAttempts = 0;
    account.UpdatedAt = DateTime.UtcNow;

    await _accountRepository.UpdateAsync(account);

    // send email
    try
    {
      var body = $"<p>Your OTP code is: <b>{otp}</b></p><p>Expires in 15 minutes.</p>";
      await _emailHandler.EmailAsync(new MentorHub.API.Utilities.EmailDto(account.Email!, "Password Change OTP", body));
    }
    catch
    {
      // swallow email errors to avoid revealing info
    }
  }

  public async Task ConfirmPasswordChangeAsync(PasswordConfirmRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp) || string.IsNullOrWhiteSpace(request.NewPassword))
      throw new InvalidOperationException("Email, OTP and new password are required");

    var account = await _accountRepository.GetByEmailAsync(request.Email, CancellationToken.None);
    if (account == null)
      throw new InvalidOperationException("Invalid OTP or email");

    if (account.OtpExpiresAt == null || account.OtpExpiresAt < DateTime.UtcNow)
      throw new InvalidOperationException("OTP expired or not requested");

    if (account.OtpAttempts >= 3)
      throw new InvalidOperationException("Maximum OTP attempts exceeded");

    var valid = _hashHandler.ValidateHash(request.Otp, account.OtpHash ?? string.Empty);
    if (!valid)
    {
      account.OtpAttempts += 1;
      await _accountRepository.UpdateAsync(account);
      throw new InvalidOperationException("Invalid OTP");
    }

    // OK - change password
    account.PasswordHash = _hashHandler.HashPassword(request.NewPassword);
    account.OtpHash = null;
    account.OtpExpiresAt = null;
    account.OtpAttempts = 0;
    account.UpdatedAt = DateTime.UtcNow;

    await _accountRepository.UpdateAsync(account);

    // send confirmation email
    try
    {
      var body = $"<p>Your password has been changed successfully.</p>";
      await _emailHandler.EmailAsync(new MentorHub.API.Utilities.EmailDto(account.Email!, "Password Changed", body));
    }
    catch
    {
    }
  }
}
