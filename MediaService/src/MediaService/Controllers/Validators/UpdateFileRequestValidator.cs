using FluentValidation;

using MediaService.Controllers.Requests;
using MediaService.FileProcessing;

namespace MediaService.Controllers.Validators;

public class UpdateFileRequestValidator : AbstractValidator<UpdateFileRequest>
{
    public UpdateFileRequestValidator()
    {
        RuleFor(x => x.BucketName).ValidBucketName();
        RuleFor(x => x.Key).ValidKey();
        
        RuleFor(x => x.File)
            .ValidFile()
            .ValidContentTypeForDestination(x => x.Destination, FileDestination.ContentImageUrl, "image/", "File must be an image for destination ContentImageUrl")
            .ValidContentTypeForDestination(x => x.Destination, FileDestination.EpisodeVideoUrl, "video/", "File must be a video for destination EpisodeVideoUrl");

        RuleFor(x => x.IsVideoProcNecessary)
            .Must(
                (
                    dto,
                    isProc
                ) => !isProc || dto.Destination == FileDestination.EpisodeVideoUrl
            )
            .WithMessage("isVideoProcNecessary can only be true if FileDestination is EpisodeVideoUrl");
    }
}
