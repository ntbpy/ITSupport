using MIT.Framework.Core.Exceptions;
using MIT.Framework.Persistence;
using MIT.Modules.Catalog.Contracts.v1.Brands;
using MIT.Modules.Catalog.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Catalog.Features.v1.Brands.RestoreBrand;

public sealed class RestoreBrandCommandHandler(CatalogDbContext dbContext)
    : ICommandHandler<RestoreBrandCommand, Guid>
{
    public async ValueTask<Guid> Handle(RestoreBrandCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        // Disable only the SoftDelete filter so we can load a deleted row;
        // tenant scoping stays in force, so cross-tenant restores cannot leak.
        var brand = await dbContext.Brands
            .IgnoreQueryFilters([QueryFilters.SoftDelete])
            .FirstOrDefaultAsync(b => b.Id == command.BrandId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Brand {command.BrandId} not found.");

        brand.Restore();
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return brand.Id;
    }
}
