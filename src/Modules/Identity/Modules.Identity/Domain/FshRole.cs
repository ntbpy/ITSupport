using Microsoft.AspNetCore.Identity;

namespace MIT.Modules.Identity.Domain;

public class FshRole : IdentityRole
{
    public string? Description { get; set; }

    public FshRole(string name, string? description = null)
        : base(name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Description = description;
        NormalizedName = name.ToUpperInvariant();
    }
}