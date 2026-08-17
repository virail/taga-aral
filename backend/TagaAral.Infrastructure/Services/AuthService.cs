using TagaAral.Core.Contracts;
using TagaAral.Infrastructure.Data;
using TagaAral.Core.Entities;
using TagaAral.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace TagaAral.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly TagaAralDbContext _db =  null!;
    private readonly JwtTokenGenerator _jwt;

    public AuthService(TagaAralDbContext db, JwtTokenGenerator jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<AuthResult> RegisterAsync(string email, string username, string password) 
    {
        bool emailExists = await _db.Users.AnyAsync(u => u.Email == email);
        if (emailExists)
        {
            throw new EmailAlreadyExistsException("Email is already registered.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User(email, username, passwordHash);
        var refreshToken = new RefreshToken(user);

        _db.Users.Add(user);
        _db.RefreshTokens.Add(refreshToken);

        await _db.SaveChangesAsync();

        string accessToken = _jwt.Generate(user);

        return new AuthResult(accessToken, refreshToken.Token);
    }
    public async Task<AuthResult> LoginAsync(string email, string password) 
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!valid)
        {
            throw new InvalidCredentialsException("Invalid email or password.");
        }

        var refreshToken = new RefreshToken(user);

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        string accessToken = _jwt.Generate(user);

        return new AuthResult(accessToken, refreshToken.Token);

    }
    public async Task<AuthResult> RefreshAsync(string refreshToken) {
            var stored = await _db.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (stored == null || !stored.IsActive)
            {
                throw new InvalidRefreshTokenException("Invalid or expired refresh token.");
            }

            string accessToken = _jwt.Generate(stored.User);

            return new AuthResult(accessToken, stored.Token);

    }
}
