using System.ComponentModel.DataAnnotations;

using AuthService.Utils.Configuration;

using JetBrains.Annotations;

namespace AuthService.Tests.Utils.Configuration;

[TestSubject(typeof(GrpcClientOptions))]
public class GrpcClientOptionsTest
{
    [Fact]
    public void Validate_WithValidContentServiceUrl_ShouldPassValidation()
    {
        // Arrange
        var options = new GrpcClientOptions
        {
            ContentServiceUrl = "https://example.com"
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void Validate_WithMissingContentServiceUrl_ShouldFailValidation()
    {
        // Arrange
        var options = new GrpcClientOptions
        {
            ContentServiceUrl = null!
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "ContentServiceUrl is required");
    }

    [Fact]
    public void Validate_WithInvalidContentServiceUrl_ShouldFailValidation()
    {
        // Arrange
        var options = new GrpcClientOptions
        {
            ContentServiceUrl = "invalid-url"
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "ContentServiceUrl must be a valid URL");
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
