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

    [Fact]
    public void Validate_NullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var validator = new MiniValidationValidateOptions<TestOptions>(null);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null, null!));
    }

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

    [Fact]
    public void Validate_InvalidOptions_ReturnsFailure()
    {
        // Arrange
        var validator = new MiniValidationValidateOptions<TestOptions>(null);
        var options = new TestOptions { RequiredValue = null };

        // Act
        var result = validator.Validate(null, options);

        // Assert
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
