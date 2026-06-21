using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace MIT.Modules.Diagnostics.Infrastructure;

public sealed class ClaudeApiOptions
{
    public const string Section = "ClaudeApi";
    public string ApiKey { get; set; } = default!;
    public string Model { get; set; } = "claude-sonnet-4-6";
    public int MaxTokens { get; set; } = 2000;
}

public interface IClaudeApiService
{
    Task<string> AnalyzeAsync(string prompt, CancellationToken ct = default);
}

public sealed partial class ClaudeApiService(
    HttpClient httpClient,
    IOptions<ClaudeApiOptions> options,
    ILogger<ClaudeApiService> logger) : IClaudeApiService
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task<string> AnalyzeAsync(string prompt, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        var request = new
        {
            model = options.Value.Model,
            max_tokens = options.Value.MaxTokens,
            temperature = 0,
            messages = new[] { new { role = "user", content = prompt } }
        };

        for (int attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                using var response = await httpClient.PostAsJsonAsync(
                    "v1/messages", request, JsonOpts, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var body = await response.Content
                    .ReadFromJsonAsync<ClaudeResponse>(JsonOpts, ct).ConfigureAwait(false);
                return body!.Content[0].Text;
            }
            catch (HttpRequestException ex) when (attempt < 2)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt + 1));
                LogRetry(logger, attempt + 1, delay, ex);
                await Task.Delay(delay, ct).ConfigureAwait(false);
            }
        }
        throw new InvalidOperationException("Claude API failed after 3 attempts");
    }

    [LoggerMessage(Level = LogLevel.Warning,
        Message = "Claude API attempt {Attempt} failed; retrying in {Delay}")]
    private static partial void LogRetry(ILogger logger, int attempt, TimeSpan delay, Exception ex);
}

internal sealed record ClaudeResponse(List<ClaudeContent> Content);
internal sealed record ClaudeContent(string Text);
