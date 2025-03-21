﻿using FluentValidation.TestHelper;

using JetBrains.Annotations;

using MediaService.Controllers.Requests;
using MediaService.Controllers.Validators;

namespace MediaService.Tests.Controllers.Validators;

[TestSubject(typeof(CreateBucketRequestValidator))]
public class CreateBucketRequestValidatorTest
{
    private readonly CreateBucketRequestValidator _validator = new();
    
    [Fact]
    public void Validate_WithValidRequest_ReturnsNoValidationError()
    {
        // Arrange
        var request = new CreateBucketRequest("my-bucket");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BucketName);
    }
}
