using FluentValidation.TestHelper;

using JetBrains.Annotations;

using MediaService.Controllers.Requests;
using MediaService.Controllers.Validators;

namespace MediaService.Tests.Controllers.Validators;

[TestSubject(typeof(GetFileRequestValidator))]
public class GetFileRequestValidatorTest
{
    private readonly GetFileRequestValidator _validator = new();
    
    [Fact]
    public void ValidateWithValidRequest_ReturnsNoValidationError()
    {
        // Arrange
        var request = new GetFileRequest("my-bucket", "folder1/file.txt");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Key);
    }
}
