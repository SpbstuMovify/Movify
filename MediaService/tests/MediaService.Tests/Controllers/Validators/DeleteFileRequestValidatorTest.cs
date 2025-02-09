using FluentValidation.TestHelper;

using JetBrains.Annotations;

using MediaService.Controllers.Requests;
using MediaService.Controllers.Validators;

namespace MediaService.Tests.Controllers.Validators;

[TestSubject(typeof(DeleteFileRequestValidator))]
public class DeleteFileRequestValidatorTest
{
    private readonly DeleteFileRequestValidator _validator = new();
    
    [Fact]
    public void Validate_WithValidRequest_ReturnsNoValidationError()
    {
        // Arrange
        var request = new DeleteFileRequest("my-bucket", "folder1/file.txt");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Key);
    }
}
