using MIT.Framework.Core.Context;
using MIT.Framework.Core.Exceptions;
using MIT.Framework.Storage.Services;
using MIT.Modules.Files.Contracts;
using MIT.Modules.Files.Contracts.v1.Commands;
using MIT.Modules.Files.Contracts.v1.DTOs;
using MIT.Modules.Files.Data;
using MIT.Modules.Files.Domain;
using MIT.Modules.Files.Features.v1.Internal;
using MIT.Modules.Files.Services;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Files.Features.v1.ChangeVisibility;

public sealed class ChangeFileVisibilityCommandHandler(
    FilesDbContext db,
    FileAccessPolicyRegistry policies,
    ICurrentUser currentUser,
    IStorageService storage)
    : ICommandHandler<ChangeFileVisibilityCommand, FileAssetDto>
{
    public async ValueTask<FileAssetDto> Handle(ChangeFileVisibilityCommand cmd, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cmd);

        if (cmd.Visibility is not (Visibility.Public or Visibility.Private))
        {
            throw new CustomException(
                $"Unknown visibility value '{cmd.Visibility}'.",
                errors: null,
                System.Net.HttpStatusCode.BadRequest);
        }

        var f = await db.FileAssets
            .FirstOrDefaultAsync(x => x.Id == cmd.FileAssetId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("file not found");

        var userId = currentUser.GetUserId().ToString();
        var policy = policies.Resolve(f.OwnerType)
            ?? throw new ForbiddenException("no policy");
        var ctx = new FileAccessContext(f.Id, f.OwnerType, f.OwnerId, f.CreatedByUserId, (int)f.Visibility);
        if (!await policy.CanChangeVisibilityAsync(ctx, userId, cancellationToken).ConfigureAwait(false))
        {
            throw new ForbiddenException("not allowed to change this file's visibility");
        }

        f.ChangeVisibility(cmd.Visibility);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var publicUrl = f.Visibility == Visibility.Public
            ? storage.BuildPublicUrl(f.StorageKey)
            : null;
        return FileAssetMapper.ToDto(f, publicUrl);
    }
}
