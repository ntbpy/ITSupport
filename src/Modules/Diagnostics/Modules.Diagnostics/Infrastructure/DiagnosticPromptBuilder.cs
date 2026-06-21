namespace MIT.Modules.Diagnostics.Infrastructure;

public static class DiagnosticPromptBuilder
{
    public static string Build(DiagnosticContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        return $$"""
        Bạn là AI chuyên gia IT với 10 năm kinh nghiệm support hệ thống Windows cho doanh nghiệp Việt Nam.

        Phân tích thông tin hệ thống sau và xác định các vấn đề cần xử lý:

        THÔNG TIN MÁY TÍNH:
        - Tên máy: {{ctx.MachineName}}
        - OS: {{ctx.OsVersion}}
        - CPU: {{ctx.CpuModel}}
        - RAM tổng: {{ctx.RamGb}}GB
        - Disk tổng: {{ctx.DiskTotalGb}}GB

        METRICS 7 NGÀY GẦN NHẤT (trung bình):
        - CPU usage trung bình: {{ctx.AvgCpu:F1}}%
        - CPU usage cao nhất: {{ctx.MaxCpu:F1}}%
        - RAM used trung bình: {{ctx.AvgRam:F1}}GB ({{ctx.AvgRamPct:F1}}%)
        - Disk used: {{ctx.DiskUsedGb:F1}}GB ({{ctx.DiskUsedPct:F1}}%)
        - Số lỗi Event Viewer trung bình/ngày: {{ctx.AvgErrors:F0}}

        PHẦN MỀM ĐÃ CÀI:
        {{ctx.SoftwareListJson}}

        TRẠNG THÁI BẢO MẬT:
        - Antivirus: {{ctx.AntivirusName}}, enabled={{ctx.AntivirusEnabled}}
        - Firewall: {{ctx.FirewallEnabled}}

        WINDOWS UPDATE:
        - Số update đang chờ: {{ctx.PendingUpdates}}

        Hãy phân tích và trả về JSON với format sau (KHÔNG trả về gì ngoài JSON):
        {
          "severity": "low|medium|high|critical",
          "issues": [{"title":"","description":"","impact":"","category":"performance|security|storage|software|hardware"}],
          "fixes": [{"title":"","description":"","steps":[],"auto_fixable":true,"fix_command":"","priority":"immediate|scheduled|optional"}],
          "summary": "Tóm tắt 2-3 câu bằng tiếng Việt"
        }
        """;
    }
}

public sealed record DiagnosticContext(
    string MachineName, string? OsVersion, string? CpuModel,
    decimal RamGb, decimal DiskTotalGb,
    decimal AvgCpu, decimal MaxCpu, decimal AvgRam, decimal AvgRamPct,
    decimal DiskUsedGb, decimal DiskUsedPct, decimal AvgErrors,
    string SoftwareListJson, string? AntivirusName, bool AntivirusEnabled,
    bool FirewallEnabled, int PendingUpdates);
