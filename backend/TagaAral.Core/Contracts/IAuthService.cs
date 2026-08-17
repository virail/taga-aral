namespace TagaAral.Core.Contracts;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string username, string password);
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RefreshAsync(string refreshToken);
}
