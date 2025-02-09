using FluentValidation;

using MediaService.Controllers.Requests;

namespace MediaService.Controllers.Validators;

public class GetFilesRequestValidator : AbstractValidator<GetFilesRequest>
{
    public GetFilesRequestValidator()
    {
        RuleFor(x => x.BucketName).ValidBucketName();
        RuleFor(x => x.Prefix).ValidPrefix();
    }
}
