using MIT.Framework.Shared.Persistence;
using MIT.Modules.Catalog.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Catalog.Contracts.v1.Categories;

public sealed record ListTrashedCategoriesQuery(int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResponse<CategoryDto>>;
