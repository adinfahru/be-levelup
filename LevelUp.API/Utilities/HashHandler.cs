namespace LevelUp.API.Utilities
{
  public interface IHashHandler
  {
    string HashPassword(string password);
    bool ValidateHash(string password, string hash);
  }

  public class HashHandler : IHashHandler
  {
    private string GenerateSalt()
    {
      return BCrypt.Net.BCrypt.GenerateSalt(15);
    }

    public string HashPassword(string password)
    {
      var salt = GenerateSalt();
      return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    public bool ValidateHash(string password, string hash)
    {
      return BCrypt.Net.BCrypt.Verify(password, hash);
    }
  }
}