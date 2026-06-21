using MIT.Modules.Diagnostics.Contracts.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MIT.Modules.Diagnostics.Infrastructure;

public sealed class DiagnosticParseException : Exception
{
    public DiagnosticParseException() { }
    public DiagnosticParseException(string message) : base(message) { }
    public DiagnosticParseException(string message, Exception? inner) : base(message, inner) { }
}

public static class DiagnosticResultParser
{
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public static DiagnosticResult Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        try
        {
            return JsonSerializer.Deserialize<DiagnosticResult>(json, Opts)
                ?? throw new DiagnosticParseException("Null result from Claude API");
        }
        catch (JsonException ex)
        {
            throw new DiagnosticParseException($"Failed to parse Claude response: {ex.Message}", ex);
        }
    }
}

public sealed record DiagnosticResult(
    DiagnosticSeverity Severity,
    IReadOnlyList<DiagnosticIssue> Issues,
    IReadOnlyList<DiagnosticFix> Fixes,
    string Summary);

public sealed record DiagnosticIssue(
    string Title, string Description, string Impact, string Category);

public sealed record DiagnosticFix(
    string Title, string Description, IReadOnlyList<string> Steps,
    bool AutoFixable,
    [property: JsonPropertyName("fix_command")] string? FixCommand,
    string Priority);
