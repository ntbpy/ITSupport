using MIT.Modules.Catalog.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Catalog.Contracts.v1.Products;

public sealed record GetProductByIdQuery(Guid ProductId) : IQuery<ProductDto>;
