using MIT.Modules.Catalog.Contracts.Dtos;
using Mediator;

namespace MIT.Modules.Catalog.Contracts.v1.Categories;

public sealed record GetCategoryTreeQuery : IQuery<IReadOnlyList<CategoryTreeNodeDto>>;
