using FluentValidation;
using FluentValidation.TestHelper;

using JetBrains.Annotations;

using MediaService.Controllers.Validators;
using MediaService.FileProcessing;

using Microsoft.AspNetCore.Http;

namespace MediaService.Tests.Controllers.Validators;

[TestSubject(typeof(ValidatorExtensions))]
public static class ValidatorExtensionsTest
{
    private class TestModel
    {
        public string? BucketName { get; init; }
        public string? Key { get; init; }
        public string? Prefix { get; init; }
        public IFormFile? File { get; init; }
        public FileDestination Destination { get; init; }
    }

    private static class TestHelpers
    {
        public static IFormFile CreateFakeFormFile(
            string fileName,
            string contentType,
            int length
        )
        {
            var content = new byte[length];
            new Random().NextBytes(content);
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }
    }

    public class ValidBucketNameTests
    {
        private class TestBucketNameValidator : AbstractValidator<TestModel>
        {
            public TestBucketNameValidator() { RuleFor(x => x.BucketName)!.ValidBucketName(); }
        }

        private readonly TestBucketNameValidator _validator = new TestBucketNameValidator();

        [Fact]
        public void ValidBucketName_WithEmptyValue_ReturnsValidationError()
        {
            // Arrange
            var model = new TestModel { BucketName = "" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BucketName)
                  .WithErrorMessage("Bucket name cannot be empty");
        }

        [Theory]
        [InlineData("MyBucket")]
        [InlineData("192.168.0.1")]
        [InlineData("bucket_with_invalid_char")]
        public void ValidBucketName_WithInvalidValue_ReturnsValidationError(string bucketName)
        {
            // Arrange
            var model = new TestModel { BucketName = bucketName };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BucketName)
                  .WithErrorMessage("Bucket name is not valid for Amazon S3");
        }

        [Theory]
        [InlineData("mybucket")]
        [InlineData("my-bucket")]
        [InlineData("my.bucket")]
        [InlineData("bucket123")]
        public void ValidBucketName_WithValidValue_ReturnsNoValidationError(string bucketName)
        {
            // Arrange
            var model = new TestModel { BucketName = bucketName };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.BucketName);
        }
    }

    public class ValidKeyTests
    {
        private class TestKeyValidator : AbstractValidator<TestModel>
        {
            public TestKeyValidator() { RuleFor(x => x.Key)!.ValidKey(); }
        }

        private readonly TestKeyValidator _validator = new TestKeyValidator();

        [Fact]
        public void ValidKey_WithEmptyValue_ReturnsValidationError()
        {
            var model = new TestModel { Key = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Key)
                  .WithErrorMessage("Key cannot be empty");
        }

        [Theory]
        [InlineData("folder1/folder2/")]
        [InlineData("/file.txt")]
        [InlineData("folder1//file.txt")]
        public void ValidKey_WithInvalidValue_ReturnsValidationError(string key)
        {
            var model = new TestModel { Key = key };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Key)
                  .WithErrorMessage("Key must be in the format \"{}/{}/.../{}\"");
        }

        [Theory]
        [InlineData("file.txt")]
        [InlineData("folder1/file.txt")]
        [InlineData("folder1/folder2/file.txt")]
        public void ValidKey_WithValidValue_ReturnsNoValidationError(string key)
        {
            var model = new TestModel { Key = key };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Key);
        }
    }

    public class ValidPrefixTests
    {
        private class TestPrefixValidator : AbstractValidator<TestModel>
        {
            public TestPrefixValidator() { RuleFor(x => x.Prefix)!.ValidPrefix(); }
        }

        private readonly TestPrefixValidator _validator = new TestPrefixValidator();

        [Fact]
        public void ValidPrefix_WithEmptyValue_ReturnsNoValidationError()
        {
            var model = new TestModel { Prefix = "" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Prefix);
        }

        [Theory]
        [InlineData("folder1/")]
        [InlineData("folder1/folder2/")]
        public void ValidPrefix_WithValidValue_ReturnsNoValidationError(string prefix)
        {
            var model = new TestModel { Prefix = prefix };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Prefix);
        }

        [Theory]
        [InlineData("folder1")]
        [InlineData("folder1/folder2")]
        public void ValidPrefix_WithInvalidValue_ReturnsValidationError(string prefix)
        {
            var model = new TestModel { Prefix = prefix };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Prefix)
                  .WithErrorMessage("Prefix must be in the format \"{}/{}/.../\"");
        }
    }

    public class ValidFileTests
    {
        private class TestFileValidator : AbstractValidator<TestModel>
        {
            public TestFileValidator() { RuleFor(x => x.File)!.ValidFile(); }
        }

        private readonly TestFileValidator _validator = new TestFileValidator();

        [Fact]
        public void ValidFile_WithZeroLengthFile_ReturnsValidationError()
        {
            var file = TestHelpers.CreateFakeFormFile("dummy.txt", "text/plain", 0);
            var model = new TestModel { File = file };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.File)
                  .WithErrorMessage("File is empty");
        }

        [Fact]
        public void ValidFile_WithValidFile_ReturnsNoValidationError()
        {
            var file = TestHelpers.CreateFakeFormFile("dummy.txt", "text/plain", 100);
            var model = new TestModel { File = file };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.File);
        }
    }

    public class ValidContentTypeForDestinationTests
    {
        private class TestContentTypeValidator : AbstractValidator<TestModel>
        {
            public TestContentTypeValidator(
                FileDestination expectedDestination,
                string contentTypePrefix,
                string errorMessage
            )
            {
                RuleFor(x => x.File)!
                    .ValidContentTypeForDestination(x => x.Destination, expectedDestination, contentTypePrefix, errorMessage);
            }
        }

        [Fact]
        public void ValidContentTypeForDestination_WhenDestinationMatchesAndContentTypeValid_ReturnsNoValidationError()
        {
            // Arrange
            var file = TestHelpers.CreateFakeFormFile("dummy.jpg", "image/jpeg", 100);
            var model = new TestModel { File = file, Destination = FileDestination.ContentImageUrl };

            var validator = new TestContentTypeValidator(
                expectedDestination: FileDestination.ContentImageUrl,
                contentTypePrefix: "image/",
                errorMessage: "File must be an image for destination ContentImageUrl."
            );

            // Act
            var result = validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.File);
        }

        [Fact]
        public void ValidContentTypeForDestination_WhenDestinationMatchesAndContentTypeInvalid_ReturnsValidationError()
        {
            // Arrange
            var file = TestHelpers.CreateFakeFormFile("dummy.txt", "text/plain", 100);
            var model = new TestModel { File = file, Destination = FileDestination.ContentImageUrl };

            var validator = new TestContentTypeValidator(
                expectedDestination: FileDestination.ContentImageUrl,
                contentTypePrefix: "image/",
                errorMessage: "File must be an image for destination ContentImageUrl."
            );

            // Act
            var result = validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.File)
                  .WithErrorMessage("File must be an image for destination ContentImageUrl.");
        }

        [Fact]
        public void ValidContentTypeForDestination_WhenDestinationDoesNotMatch_ReturnsNoValidationErrorRegardlessOfContentType()
        {
            // Arrange
            var file = TestHelpers.CreateFakeFormFile("dummy.txt", "text/plain", 100);
            var model = new TestModel { File = file, Destination = FileDestination.EpisodeVideoUrl };

            var validator = new TestContentTypeValidator(
                expectedDestination: FileDestination.ContentImageUrl,
                contentTypePrefix: "image/",
                errorMessage: "File must be an image for destination ContentImageUrl."
            );

            // Act
            var result = validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.File);
        }
    }
}
