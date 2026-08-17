namespace TagaAral.Core.Exceptions;

public class InvalidRefreshTokenException : Exception
{
    public InvalidRefreshTokenException(string message) : base(message) {}
}

