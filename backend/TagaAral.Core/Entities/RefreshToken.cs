namespace TagaAral.Core.Entities;

using System.Security.Cryptography;

public class RefreshToken 
{
    private RefreshToken() { }

    public RefreshToken(User user)
    {
        User = user;
        UserId = user.Id;
        Token = CreateRandomToken();
        CreatedAtUtc = DateTimeOffset.UtcNow;
        ExpiresAtUtc = CreatedAtUtc.AddDays(7);
        RevokedAtUtc = null;
    }

    public Guid Id { get; private set; }
    public string Token { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public User User { get; private set; } = null!;

    public bool IsActive =>
        RevokedAtUtc == null && ExpiresAtUtc > DateTimeOffset.UtcNow;

    public void Revoke()
    {
        RevokedAtUtc = DateTimeOffset.UtcNow;
    }

    private static string CreateRandomToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

}
