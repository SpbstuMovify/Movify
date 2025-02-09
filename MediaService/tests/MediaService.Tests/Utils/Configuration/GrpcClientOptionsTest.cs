using System.ComponentModel.DataAnnotations;

using JetBrains.Annotations;

using MediaService.Utils.Configuration;

namespace MediaService.Tests.Utils.Configuration;

[TestSubject(typeof(GrpcClientOptions))]
public class GrpcClientOptionsTest
{

    [Fact]
    public void Validate_WithValidServiceUrls_PassValidation()
    {
        // Arrange
        var options = new GrpcClientOptions
        {
            AuthServiceUrl = "https://example.com",
            ChunkerServiceUrl = "https://example.com",
            ContentServiceUrl = "https://example.com"
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
            AuthServiceUrl = null!,
            ChunkerServiceUrl = null!,
            ContentServiceUrl = null!
        };
    
        // Act
        var validationResults = ValidateModel(options);
    
        // Assert
        Assert.Equal(3, validationResults.Count);
        Assert.Contains(validationResults, r => r.ErrorMessage == "AuthServiceUrl is required");
        Assert.Contains(validationResults, r => r.ErrorMessage == "ChunkerServiceUrl is required");
        Assert.Contains(validationResults, r => r.ErrorMessage == "ContentServiceUrl is required");
    }
    
    [Fact]
    public void Validate_WithInvalidContentServiceUrl_FailValidation()
    {
        // Arrange
        var options = new GrpcClientOptions
        {
            AuthServiceUrl = "invalid-url",
            ChunkerServiceUrl = "invalid-url",
            ContentServiceUrl = "invalid-url"
        };
    
        // Act
        var validationResults = ValidateModel(options);
    
        // Assert
        Assert.Equal(3, validationResults.Count);
        Assert.Contains(validationResults, r => r.ErrorMessage == "AuthServiceUrl must be a valid URL");
        Assert.Contains(validationResults, r => r.ErrorMessage == "ChunkerServiceUrl must be a valid URL");
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
