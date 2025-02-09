using FluentValidation;

using MediaService.Controllers.Requests;

namespace MediaService.Controllers.Validators;

public class DeleteBucketRequestValidator : AbstractValidator<DeleteBucketRequest>
{
    public DeleteBucketRequestValidator()
    {
        RuleFor(x => x.BucketName).ValidBucketName();
    }
}
