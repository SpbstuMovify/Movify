using FluentValidation.TestHelper;

using JetBrains.Annotations;

using MediaService.Controllers.Requests;
using MediaService.Controllers.Validators;
using MediaService.FileProcessing;

using Microsoft.AspNetCore.Http;

namespace MediaService.Tests.Controllers.Validators;

[TestSubject(typeof(UpdateFileRequestValidator))]
public class UpdateFileRequestValidatorTest
{
    private readonly UpdateFileRequestValidator _validator = new();

    [Theory]
    [InlineData(FileDestination.Internal)]
    [InlineData(FileDestination.ContentImageUrl)]
    public void Validate_WithIsVideoProcNecessaryTrueButWrongDestination_ReturnsValidationError(FileDestination destination)
    {
        // Arrange
        var file = CreateFakeFile("dummy.txt", "text/plain", 100);
        var request = new UpdateFileRequest(
            File: file,
            BucketName: "mybucket",
            Key: "folder1/dummy.txt",
            IsVideoProcNecessary: true,
            Destination: destination
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IsVideoProcNecessary)
              .WithErrorMessage("isVideoProcNecessary can only be true if FileDestination is EpisodeVideoUrl");
    }

    [Fact]
    public void Validate_WithDestinationContentImageUrlButNonImageFile_ReturnsValidationError()
    {
        // Arrange
        var file = CreateFakeFile("dummy.txt", "video/mp4", 100);
        var request = new UpdateFileRequest(
            File: file,
            BucketName: "mybucket",
            Key: "folder1/dummy.txt",
            IsVideoProcNecessary: false,
            Destination: FileDestination.ContentImageUrl
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File)
              .WithErrorMessage("File must be an image for destination ContentImageUrl");
    }

    [Fact]
    public void Validate_WithDestinationContentImageUrlAndImageFile_ReturnsNoValidationError()
    {
        // Arrange
        var file = CreateFakeFile("dummy.jpg", "image/jpeg", 100);
        var request = new UpdateFileRequest(
            File: file,
            BucketName: "mybucket",
            Key: "folder1/dummy.txt",
            IsVideoProcNecessary: false,
            Destination: FileDestination.ContentImageUrl
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File.ContentType);
    }

    [Fact]
    public void Validate_WithDestinationEpisodeVideoUrlButNonVideoFile_ReturnsValidationError()
    {
        // Arrange
        var file = CreateFakeFile("dummy.png", "image/png", 100);
        var request = new UpdateFileRequest(
            File: file,
            BucketName: "mybucket",
            Key: "folder1/",
            IsVideoProcNecessary: false,
            Destination: FileDestination.EpisodeVideoUrl
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.File)
              .WithErrorMessage("File must be a video for destination EpisodeVideoUrl");
    }

    [Fact]
    public void Validate_WithDestinationEpisodeVideoUrlAndVideoFile_ReturnsNoValidationError()
    {
        // Arrange
        var file = CreateFakeFile("dummy.mp4", "video/mp4", 100);
        var request = new UpdateFileRequest(
            File: file,
            BucketName: "mybucket",
            Key: "folder1/dummy.txt",
            IsVideoProcNecessary: false,
            Destination: FileDestination.EpisodeVideoUrl
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
    }

    [Fact]
    public void Validate_WithValidRequest_ReturnsNoValidationError()
    {
        // Arrange
        var file = CreateFakeFile("dummy.txt", "text/plain", 100);
        var request = new UpdateFileRequest(
            File: file,
            BucketName: "mybucket",
            Key: "folder1/dummy.txt",
            IsVideoProcNecessary: false,
            Destination: FileDestination.Internal
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.File);
        result.ShouldNotHaveValidationErrorFor(x => x.BucketName);
        result.ShouldNotHaveValidationErrorFor(x => x.Key);
        result.ShouldNotHaveValidationErrorFor(x => x.IsVideoProcNecessary);
    }

    private static FormFile CreateFakeFile(
        string fileName,
        string contentType,
        int length
    )
    {
        var fileContent = new byte[length];
        var stream = new MemoryStream(fileContent);

        var formFile = new FormFile(stream, 0, length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

        return formFile;
    }
}
