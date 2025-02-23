using AuthService.Utils.Exceptions;

using JetBrains.Annotations;

namespace AuthService.Tests.Utils.Exceptions;

[TestSubject(typeof(AuthServiceException))]
public class AuthServiceExceptionTest
{

    [Fact]
    public void DefaultConstructor_ShouldCreateInstanceWithDefaultMessage()
    {
        // Arrange & Act
        var exception = new AuthServiceException();

        // Assert
        Assert.IsType<AuthServiceException>(exception);
        Assert.False(string.IsNullOrEmpty(exception.Message));
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void MessageConstructor_ShouldSetMessageProperty()
    {
        // Arrange
        const string expectedMessage = "Test error message";

        // Act
        var exception = new AuthServiceException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void MessageAndInnerExceptionConstructor_ShouldSetProperties()
    {
        // Arrange
        const string expectedMessage = "Test error message";
        var innerException = new Exception("Inner exception message");

        // Act
        var exception = new AuthServiceException(expectedMessage, innerException);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }
}
