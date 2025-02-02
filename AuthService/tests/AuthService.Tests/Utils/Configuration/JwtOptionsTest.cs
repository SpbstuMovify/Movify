using System.ComponentModel.DataAnnotations;

using AuthService.Utils.Configuration;

using JetBrains.Annotations;

namespace AuthService.Tests.Utils.Configuration;

[TestSubject(typeof(JwtOptions))]
public class JwtOptionsTest
{
    [Fact]
    public void Validate_WithValidJwtOptions_ShouldPassValidation()
    {
        // Arrange
        var options = new JwtOptions
        {
            Secret = "1234567890123456",
            ExpirySeconds = 3600
        };
        
        // Act
        var validationResults = ValidateModel(options);
        
        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void Validate_WithMissingSecret_ShouldFailValidation()
    {
        // Arrange
        var options = new JwtOptions
        {
            Secret = null!,
            ExpirySeconds = 3600
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "Secret is required");
    }

    [Fact]
    public void Validate_WithShortSecret_ShouldFailValidation()
    {
        // Arrange
        var options = new JwtOptions
        {
            Secret = "short",
            ExpirySeconds = 3600
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "Secret must be at least 16 characters long");
    }

    [Fact]
    public void Validate_WithNegativeExpirySeconds_ShouldFailValidation()
    {
        // Arrange
        var options = new JwtOptions
        {
            Secret = "1234567890123456",
            ExpirySeconds = -1
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "ExpirySeconds must be a positive integer");
    }

    [Fact]
    public void Validate_WithZeroExpirySeconds_ShouldFailValidation()
    {
        // Arrange
        var options = new JwtOptions
        {
            Secret = "1234567890123456",
            ExpirySeconds = 0
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "ExpirySeconds must be a positive integer");
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
