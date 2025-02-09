using System.Text.RegularExpressions;

using FluentValidation;

using MediaService.FileProcessing;

namespace MediaService.Controllers.Validators;

public static partial class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> ValidBucketName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
               .NotEmpty().WithMessage("Bucket name cannot be empty")
               .Matches(S3BucketNamePattern)
               .WithMessage("Bucket name is not valid for Amazon S3");
    }

    public static IRuleBuilderOptions<T, string> ValidKey<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
               .NotEmpty().WithMessage("Key cannot be empty")
               .Matches(KeyPattern)
               .WithMessage("Key must be in the format \"{}/{}/.../{}\"");
    }

    public static IRuleBuilderOptions<T, string> ValidPrefix<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
               .Must(prefix => string.IsNullOrEmpty(prefix) || PrefixRegex().IsMatch(prefix))
               .WithMessage("Prefix must be in the format \"{}/{}/.../\"");
    }

    public static IRuleBuilderOptions<T, IFormFile> ValidFile<T>(this IRuleBuilder<T, IFormFile> ruleBuilder)
    {
        return ruleBuilder
               .NotNull().WithMessage("File is not selected")
               .Must(file => file.Length > 0).WithMessage("File is empty");
    }

    public static IRuleBuilderOptions<T, IFormFile> ValidContentTypeForDestination<T>(
        this IRuleBuilder<T, IFormFile> ruleBuilder,
        Func<T, FileDestination> destinationSelector,
        FileDestination expectedDestination,
        string contentTypePrefix,
        string errorMessage
    )
    {
        return ruleBuilder.Must(
            (
                instance,
                file
            ) => destinationSelector(instance) != expectedDestination || file.ContentType.StartsWith(contentTypePrefix, StringComparison.OrdinalIgnoreCase)
        ).WithMessage(errorMessage);
    }

    private const string S3BucketNamePattern = @"^(?!.*\.\.)(?!.*\.$)(?!^\d+\.\d+\.\d+\.\d+$)[a-z0-9]([a-z0-9.-]{1,61}[a-z0-9])?$";
    private const string PrefixPattern = @"^(?:[^/]+/)+$";
    private const string KeyPattern = @"^(?:[^/]+/)*[^/]+$";

    [GeneratedRegex(PrefixPattern)]
    private static partial Regex PrefixRegex();
}
