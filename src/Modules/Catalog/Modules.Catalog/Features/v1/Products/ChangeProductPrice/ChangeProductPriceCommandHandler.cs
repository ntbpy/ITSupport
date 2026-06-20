using MIT.Framework.Core.Exceptions;
using MIT.Modules.Catalog.Contracts.v1.Products;
using MIT.Modules.Catalog.Data;
using MIT.Modules.Catalog.Domain;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace MIT.Modules.Catalog.Features.v1.Products.ChangeProductPrice;

public sealed class ChangeProductPriceCommandHandler(CatalogDbContext dbContext)
    : ICommandHandler<ChangeProductPriceCommand, Guid>
{
    public async ValueTask<Guid> Handle(ChangeProductPriceCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var product = await dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == command.ProductId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Product {command.ProductId} not found.");

        product.ChangePrice(new Money(command.Amount, command.Currency));
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return product.Id;
    }
}
