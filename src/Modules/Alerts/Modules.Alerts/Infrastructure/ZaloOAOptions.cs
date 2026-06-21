namespace MIT.Modules.Alerts.Infrastructure;

public sealed class ZaloOAOptions
{
    public const string Section = "ZaloOA";
    public string AccessToken { get; set; } = default!;
    public string RecipientId { get; set; } = default!;
    public string DashboardUrl { get; set; } = "https://app.vietrmm.vn";
}
