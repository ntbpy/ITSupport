using MIT.Framework.Shared.Constants;

namespace MIT.Modules.Alerts.Contracts.Authorization;

public static class AlertsPermissions
{
    public static class Alerts
    {
        public const string Resource = "Alerts";
        public const string View    = $"Permissions.{Resource}.View";
        public const string Acknowledge = $"Permissions.{Resource}.Acknowledge";
    }

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View Alerts",        ActionConstants.View,   Alerts.Resource, IsBasic: true),
        new("Acknowledge Alerts", "Acknowledge",          Alerts.Resource),
    ];
}
