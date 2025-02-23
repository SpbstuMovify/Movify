using AuthService.Utils.Exceptions;

using JetBrains.Annotations;

namespace AuthService.Tests.Utils.Exceptions;

[TestSubject(typeof(InvalidCredentialsException))]
public class InvalidCredentialsExceptionTest
{
    [Fact]
    public void DefaultConstructor_ShouldCreateInstanceWithDefaultMessage()
    {
        // Arrange & Act
        var exception = new InvalidCredentialsException();

        // Assert
        Assert.IsType<InvalidCredentialsException>(exception);
        // По умолчанию Message не должен быть пустым (хотя стандартное сообщение может зависеть от реализации Exception)
        Assert.False(string.IsNullOrEmpty(exception.Message));
        // Для конструктора по умолчанию InnerException должна быть null.
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void MessageConstructor_ShouldPrefixMessageAndSetMessageProperty()
    {
        // Arrange
        const string inputMessage = "User not found";
        const string expectedMessage = $"Invalid credentials provided: {inputMessage}";

        // Act
        var exception = new InvalidCredentialsException(inputMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        // В данном конструкторе вложенное исключение не передаётся.
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void MessageAndInnerExceptionConstructor_ShouldPrefixMessageAndSetInnerException()
    {
        // Arrange
        const string inputMessage = "Password incorrect";
        const string expectedMessage = $"Invalid credentials provided: {inputMessage}";
        var innerException = new Exception("Inner error message");

        // Act
        var exception = new InvalidCredentialsException(inputMessage, innerException);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal(innerException, exception.InnerException);
    }
}
