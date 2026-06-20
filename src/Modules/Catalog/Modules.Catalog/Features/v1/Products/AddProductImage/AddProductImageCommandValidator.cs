using FluentValidation;
using MIT.Modules.Catalog.Contracts.v1.Products.AddProductImage;

namespace MIT.Modules.Catalog.Features.v1.Products.AddProductImage;

public sealed class AddProductImageCommandValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Url).NotEmpty().MaximumLength(2048);
    }
}
