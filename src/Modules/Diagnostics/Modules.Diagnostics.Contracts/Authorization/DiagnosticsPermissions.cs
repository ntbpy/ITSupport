using MIT.Framework.Shared.Constants;

namespace MIT.Modules.Diagnostics.Contracts.Authorization;

public static class DiagnosticsPermissions
{
    public static class Diagnostics
    {
        public const string Resource = "Diagnostics";
        public const string View    = $"Permissions.{Resource}.View";
        public const string Trigger = $"Permissions.{Resource}.Trigger";
    }

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View Diagnostics",    ActionConstants.View,   Diagnostics.Resource, IsBasic: true),
        new("Trigger Diagnostics", "Trigger",              Diagnostics.Resource),
    ];
}
