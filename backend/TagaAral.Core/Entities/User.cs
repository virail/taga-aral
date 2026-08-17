namespace TagaAral.Core.Entities;

public class User 
{
    private User()
    {
    }

    public User(string email, string username, string passwordHash)
    {
        Email = email;
        Username = username;
        PasswordHash = passwordHash;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string Username { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public ICollection<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();

    public void UpdateProfile(string email, string username)
    {
        Email = email;
        Username = username;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
