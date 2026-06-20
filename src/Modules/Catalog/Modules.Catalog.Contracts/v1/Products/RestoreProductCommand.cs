using Mediator;

namespace MIT.Modules.Catalog.Contracts.v1.Products;

public sealed record RestoreProductCommand(Guid ProductId) : ICommand<Guid>;
