using FluentValidation;

using MediaService.Controllers.Requests;

namespace MediaService.Controllers.Validators;

public class DeleteFileRequestValidator : AbstractValidator<DeleteFileRequest>
{
    public DeleteFileRequestValidator()
    {
        RuleFor(x => x.BucketName).ValidBucketName();
        RuleFor(x => x.BucketName).ValidKey();
    }
}
