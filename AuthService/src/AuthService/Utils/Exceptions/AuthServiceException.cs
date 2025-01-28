namespace AuthService.Utils.Exceptions;

public class AuthServiceException : Exception
{
    public AuthServiceException() { }

    public AuthServiceException(string message) : base(message) { }

    public AuthServiceException(
        string message,
        Exception innerException
    ) : base(message, innerException) { }
}
