using MIT.Framework.Shared.Constants;

namespace MIT.Modules.Machines.Contracts.Authorization;

public static class MachinesPermissions
{
    public static class Machines
    {
        public const string Resource = "Machines";
        public const string View    = $"Permissions.{Resource}.View";
        public const string Update  = $"Permissions.{Resource}.Update";
        public const string Command = $"Permissions.{Resource}.Command";
    }

    public static IReadOnlyList<FshPermission> All { get; } =
    [
        new("View Machines",  ActionConstants.View,   Machines.Resource, IsBasic: true),
        new("Update Machines", ActionConstants.Update, Machines.Resource),
        new("Send Commands",   "Command",              Machines.Resource),
    ];
}
