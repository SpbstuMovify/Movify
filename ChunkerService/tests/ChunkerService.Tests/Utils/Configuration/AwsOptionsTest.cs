using System.ComponentModel.DataAnnotations;

using ChunkerService.Utils.Configuration;

using JetBrains.Annotations;

namespace ChunkerService.Tests.Utils.Configuration;

[TestSubject(typeof(AwsOptions))]
public class AwsOptionsTest
{
    [Fact]
    public void Validate_WithValidServiceUrls_PassValidation()
    {
        // Arrange
        var options = new AwsOptions
        {
            AccessKey = "AccessKey",
            SecretKey = "SecretKey",
            Region = "us-east-1",
            ServiceUrl = "https://example.com",
            UsePathStyle = true
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
        var options = new AwsOptions
        {
            AccessKey = null,
            SecretKey = null,
            Region = null!,
            ServiceUrl = null!,
            UsePathStyle = true
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, r => r.ErrorMessage == "Region is required");
        Assert.Contains(validationResults, r => r.ErrorMessage == "ServiceUrl is required");
    }

    [Fact]
    public void Validate_WithInvalidContentServiceUrl_FailValidation()
    {
        // Arrange
        var options = new AwsOptions
        {
            AccessKey = "AccessKey",
            SecretKey = "SecretKey",
            Region = "us-east-1",
            ServiceUrl = "invalid-url",
            UsePathStyle = true
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "ServiceUrl must be a valid URL");
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
