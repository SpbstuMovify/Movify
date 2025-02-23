using System.ComponentModel.DataAnnotations;

using ChunkerService.Utils.Configuration;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace ChunkerService.Tests.Utils.Configuration;

[TestSubject(typeof(MiniValidationValidateOptions<TestOptions>))]
public class MiniValidationValidateOptionsTest
{
[Fact]
    public void Validate_NameMismatch_ReturnsSkip()
    {
        // Arrange
        var validator = new MiniValidationValidateOptions<TestOptions>("ExpectedName");
        var options = new TestOptions { RequiredValue = "SomeValue" };

        // Act
        var result = validator.Validate("OtherName", options);

        // Assert
        Assert.Equal(ValidateOptionsResult.Skip, result);
    }

    // Если options равен null, должен выбрасываться ArgumentNullException.
    [Fact]
    public void Validate_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var validator = new MiniValidationValidateOptions<TestOptions>(null);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null, null));
    }

    // Если валидатор создан без имени (Name == null) и options корректны,
    // должен возвращаться результат Success.
    [Fact]
    public void Validate_ValidOptions_ReturnsSuccess_WhenNameIsNull()
    {
        // Arrange
        var validator = new MiniValidationValidateOptions<TestOptions>(null);
        var options = new TestOptions { RequiredValue = "ValidValue" };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        Assert.Equal(ValidateOptionsResult.Success, result);
    }

    // Если валидатор создан с именем и переданное имя совпадает, а options корректны,
    // должен возвращаться результат Success.
    [Fact]
    public void Validate_ValidOptions_ReturnsSuccess_WhenNameMatches()
    {
        // Arrange
        var validator = new MiniValidationValidateOptions<TestOptions>("TestOptions");
        var options = new TestOptions { RequiredValue = "ValidValue" };

        // Act
        var result = validator.Validate("TestOptions", options);

        // Assert
        Assert.Equal(ValidateOptionsResult.Success, result);
    }

    // Если options не проходят валидацию (например, обязательное поле не задано),
    // должен возвращаться результат Fail с соответствующим сообщением об ошибке.
    [Fact]
    public void Validate_InvalidOptions_ReturnsFailure()
    {
        // Arrange
        var validator = new MiniValidationValidateOptions<TestOptions>(null);
        // Оставляем RequiredValue равным null, чтобы валидация не прошла
        var options = new TestOptions { RequiredValue = null };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        // Предполагается, что для невалидных опций Succeeded == false,
        // а Failure (или Failures) содержит сообщение об ошибке.
        Assert.False(result.Succeeded);
        Assert.NotNull(result.Failures);
        Assert.Contains("DataAnnotation validation failed for '' member: 'RequiredValue' with errors: 'The RequiredValue field is required.'.", result.Failures); 
    }
}

public class TestOptions
{
    [Required]
    public string? RequiredValue { get; set; }
}
