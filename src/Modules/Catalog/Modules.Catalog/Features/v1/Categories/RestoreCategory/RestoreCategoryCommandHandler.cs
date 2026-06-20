using MIT.Framework.Core.Exceptions;
using MIT.Framework.Persistence;
using MIT.Modules.Catalog.Contracts.v1.Categories;
using MIT.Modules.Catalog.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Catalog.Features.v1.Categories.RestoreCategory;

public sealed class RestoreCategoryCommandHandler(CatalogDbContext dbContext)
    : ICommandHandler<RestoreCategoryCommand, Guid>
{
    public async ValueTask<Guid> Handle(RestoreCategoryCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var category = await dbContext.Categories
            .IgnoreQueryFilters([QueryFilters.SoftDelete])
            .FirstOrDefaultAsync(c => c.Id == command.CategoryId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Category {command.CategoryId} not found.");

        category.Restore();
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return category.Id;
    }
}
