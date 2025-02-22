using System.ComponentModel.DataAnnotations;

using ChunkerService.Utils.Configuration;

using JetBrains.Annotations;

namespace ChunkerService.Tests.Utils.Configuration;

[TestSubject(typeof(HlsVariantOptions))]
public class HlsVariantOptionsTest
{
    [Fact]
    public void Validate_WithValidHlsVariant_PassValidation()
    {
        // Arrange
        var variant = new HlsVariantOptions
        {
            Name = "1080p",
            Width = 1920,
            Height = 1080,
            VideoBitrate = 5000
        };

        // Act
        var validationResults = ValidateModel(variant);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void Validate_WithMissingName_FailValidation()
    {
        // Arrange
        var variant = new HlsVariantOptions
        {
            Name = null!,
            Width = 1920,
            Height = 1080,
            VideoBitrate = 5000
        };

        // Act
        var validationResults = ValidateModel(variant);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "Name is required");
    }

    [Fact]
    public void Validate_WithInvalidNumericalFields_FailValidation()
    {
        // Arrange
        var variant = new HlsVariantOptions
        {
            Name = "1080p",
            Width = 0,
            Height = 0,
            VideoBitrate = 0
        };

        // Act
        var validationResults = ValidateModel(variant);

        // Assert
        Assert.Equal(3, validationResults.Count);
        Assert.Contains(validationResults, r => r.ErrorMessage == "Width must be greater than 0");
        Assert.Contains(validationResults, r => r.ErrorMessage == "Height must be greater than 0");
        Assert.Contains(validationResults, r => r.ErrorMessage == "VideoBitrate must be greater than 0");

    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
