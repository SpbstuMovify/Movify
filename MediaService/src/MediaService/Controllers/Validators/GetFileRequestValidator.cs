using FluentValidation;

using MediaService.Controllers.Requests;

namespace MediaService.Controllers.Validators;

public class GetFileRequestValidator : AbstractValidator<GetFileRequest>
{
    public GetFileRequestValidator()
    {
        RuleFor(x => x.BucketName).ValidBucketName();
        RuleFor(x => x.BucketName).ValidKey();
    }
}
