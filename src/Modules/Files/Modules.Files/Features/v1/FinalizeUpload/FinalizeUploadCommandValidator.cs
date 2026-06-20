using FluentValidation;
using MIT.Modules.Files.Contracts.v1.Commands;

namespace MIT.Modules.Files.Features.v1.FinalizeUpload;

public sealed class FinalizeUploadCommandValidator : AbstractValidator<FinalizeUploadCommand>
{
    public FinalizeUploadCommandValidator()
    {
        RuleFor(x => x.FileAssetId).NotEmpty();
    }
}
