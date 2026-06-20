using MIT.Framework.Shared.Persistence;
using MIT.Modules.Catalog.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Catalog.Contracts.v1.Products;

public sealed record ListTrashedProductsQuery(int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResponse<ProductDto>>;
