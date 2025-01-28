namespace AuthService.Utils.Exceptions;

public class InvalidCredentialsException : AuthServiceException
{
    public InvalidCredentialsException() { }

    public InvalidCredentialsException(string message) : base($"Invalid credentials provided: {message}") { }

    public InvalidCredentialsException(
        string message,
        Exception innerException
    ) : base($"Invalid credentials provided: {message}", innerException) { }
}
