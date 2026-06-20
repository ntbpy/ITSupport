using FluentValidation;
using MIT.Framework.Web.Validation;
using MIT.Modules.Multitenancy.Contracts.v1.GetTenants;

namespace MIT.Modules.Multitenancy.Features.v1.GetTenants;

public sealed class GetTenantsQueryValidator : AbstractValidator<GetTenantsQuery>
{
    public GetTenantsQueryValidator()
    {
        Include(new PagedQueryValidator<GetTenantsQuery>());
    }
}