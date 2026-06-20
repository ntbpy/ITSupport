using Mediator;

namespace MIT.Modules.Catalog.Contracts.v1.Categories;

public sealed record RestoreCategoryCommand(Guid CategoryId) : ICommand<Guid>;
