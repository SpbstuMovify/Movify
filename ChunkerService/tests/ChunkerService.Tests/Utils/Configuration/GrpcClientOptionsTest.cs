using System.ComponentModel.DataAnnotations;

using ChunkerService.Utils.Configuration;

using JetBrains.Annotations;

namespace ChunkerService.Tests.Utils.Configuration;

[TestSubject(typeof(GrpcClientOptions))]
public class GrpcClientOptionsTest
{

    [Fact]
    public void Validate_WithValidServiceUrls_PassValidation()
    {
        // Arrange
        var options = new GrpcClientOptions
        {
            MediaServiceUrl = "https://example.com"
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Empty(validationResults);
    }
    
    [Fact]
    public void Validate_WithMissingContentServiceUrl_FailValidation()
    {
        // Arrange
        var options = new GrpcClientOptions
        {
            MediaServiceUrl = null!
        };
    
        // Act
        var validationResults = ValidateModel(options);
    
        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "MediaServiceUrl is required");
    }
    
    [Fact]
    public void Validate_WithInvalidContentServiceUrl_FailValidation()
    {
        // Arrange
        var options = new GrpcClientOptions
        {
            MediaServiceUrl = "invalid-url"
        };
    
        // Act
        var validationResults = ValidateModel(options);
    
        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "MediaServiceUrl must be a valid URL");
    }

    
    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
