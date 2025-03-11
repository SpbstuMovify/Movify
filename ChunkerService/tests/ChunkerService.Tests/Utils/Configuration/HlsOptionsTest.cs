using System.ComponentModel.DataAnnotations;

using ChunkerService.Utils.Configuration;

using JetBrains.Annotations;

namespace ChunkerService.Tests.Utils.Configuration;

[TestSubject(typeof(HlsOptions))]
[TestSubject(typeof(HlsOptions))]
public class HlsOptionsTest
{
    [Fact]
    public void Validate_WithValidHlsOptions_PassValidation()
    {
        // Arrange
        var validVariant = new HlsVariantOptions
        {
            Name = "1080p",
            Width = 1920,
            Height = 1080,
            VideoBitrate = 5000
        };

        var options = new HlsOptions
        {
            FfmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe",
            Variants = [validVariant],
            SegmentDuration = 10,
            AudioBitrate = 128,
            AdditionalFfmpegArgs = "-someArg"
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void Validate_WithMissingFields_FailValidation()
    {
        // Arrange
        var options = new HlsOptions
        {
            FfmpegPath = null!,
            Variants = null!,
            SegmentDuration = 10,
            AudioBitrate = 128
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, r => r.ErrorMessage == "FfmpegPath is required");
        Assert.Contains(validationResults, r => r.ErrorMessage == "Variants is required");
    }
    
    [Fact]
    public void Validate_WithEmptyVariants_FailValidation()
    {
        // Arrange
        var options = new HlsOptions
        {
            FfmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe",
            Variants = [],
            SegmentDuration = 10,
            AudioBitrate = 128
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Single(validationResults);
        Assert.Contains(validationResults, r => r.ErrorMessage == "At least one variant is required");
    }

    [Fact]
    public void Validate_WithInvalidNumericalFields_FailValidation()
    {
        // Arrange
        var validVariant = new HlsVariantOptions
        {
            Name = "1080p",
            Width = 1920,
            Height = 1080,
            VideoBitrate = 5000
        };

        var options = new HlsOptions
        {
            FfmpegPath = @"C:\ffmpeg\bin\ffmpeg.exe",
            Variants = [validVariant],
            SegmentDuration = 0,
            AudioBitrate = 0
        };

        // Act
        var validationResults = ValidateModel(options);

        // Assert
        Assert.Equal(2, validationResults.Count);
        Assert.Contains(validationResults, r => r.ErrorMessage == "SegmentDuration must be greater than 0");
        Assert.Contains(validationResults, r => r.ErrorMessage == "AudioBitrate must be greater than 0");

    }
    
    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
