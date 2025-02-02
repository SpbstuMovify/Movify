namespace AuthService.Utils.Exceptions;

public class TokenValidationException : AuthServiceException
{
    public TokenValidationException() { }

    public TokenValidationException(string message) : base($"Token validation failed: {message}") { }

    public TokenValidationException(
        string message,
        Exception innerException
    ) : base($"Token validation failed: {message}", innerException) { }
}
