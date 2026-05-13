using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace HoroscopeBot.Infrastructure.AI;

public class AnthropicClient : IHoroscopeAiClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly ILogger<AnthropicClient> _logger;

    public AnthropicClient(HttpClient http, string apiKey, string model, ILogger<AnthropicClient> logger)
    {
        _http = http;
        _apiKey = apiKey;
        _model = model;
        _logger = logger;
    }

    public async Task<string> GenerateHoroscopeAsync(string prompt, CancellationToken ct = default)
    {
        var requestBody = new
        {
            model = _model,
            max_tokens = 1000,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "v1/messages")
        {
            Content = content,
            Headers = { { "x-api-key", _apiKey }, { "anthropic-version", "2023-06-01" } }
        };

        var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(responseJson);
        var text = doc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString();

        return text ?? "Failed to generate horoscope";
    }
}
