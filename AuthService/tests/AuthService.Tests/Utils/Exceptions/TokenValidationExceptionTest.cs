using AuthService.Utils.Exceptions;

using JetBrains.Annotations;

namespace AuthService.Tests.Utils.Exceptions;

[TestSubject(typeof(TokenValidationException))]
public class TokenValidationExceptionTest
{
    [Fact]
    public void DefaultConstructor_ShouldCreateInstanceWithDefaultMessage()
    {
        // Arrange & Act
        var exception = new TokenValidationException();

        // Assert
        Assert.IsType<TokenValidationException>(exception);
        Assert.False(string.IsNullOrEmpty(exception.Message));
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void MessageConstructor_ShouldPrefixMessageAndSetMessageProperty()
    {
        // Arrange
        const string inputMessage = "Invalid token format";
        const string expectedMessage = $"Token validation failed: {inputMessage}";

        // Act
        var exception = new TokenValidationException(inputMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void MessageAndInnerExceptionConstructor_ShouldPrefixMessageAndSetInnerException()
    {
        // Arrange
        const string inputMessage = "Token expired";
        const string expectedMessage = $"Token validation failed: {inputMessage}";
        var innerException = new Exception("Inner exception message");

        // Act
        var exception = new TokenValidationException(inputMessage, innerException);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }
}
