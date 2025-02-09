using FluentValidation.TestHelper;

using JetBrains.Annotations;

using MediaService.Controllers.Requests;
using MediaService.Controllers.Validators;

namespace MediaService.Tests.Controllers.Validators;

[TestSubject(typeof(DeleteBucketRequestValidator))]
public class DeleteBucketRequestValidatorTest
{
    private readonly DeleteBucketRequestValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ReturnsNoValidationError()
    {
        // Arrange
        var request = new DeleteBucketRequest("my-bucket");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BucketName);
    }
}
