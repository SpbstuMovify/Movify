using FluentValidation;

using MediaService.Controllers.Requests;

namespace MediaService.Controllers.Validators;

public class CreateBucketRequestValidator : AbstractValidator<CreateBucketRequest>
{
    public CreateBucketRequestValidator()
    {
        RuleFor(x => x.BucketName).ValidBucketName();
    }
}
