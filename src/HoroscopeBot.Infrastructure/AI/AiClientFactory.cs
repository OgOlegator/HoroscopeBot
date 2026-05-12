using Microsoft.Extensions.Logging;

namespace HoroscopeBot.Infrastructure.AI;

public class AiClientFactory
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILoggerFactory _loggerFactory;
    private readonly AiOptions _options;

    public AiClientFactory(IHttpClientFactory httpFactory, ILoggerFactory loggerFactory, AiOptions options)
    {
        _httpFactory = httpFactory;
        _loggerFactory = loggerFactory;
        _options = options;
    }

    public IHoroscopeAiClient CreatePrimary()
    {
        return CreateClient(_options.PrimaryProvider);
    }

    public IHoroscopeAiClient CreateSecondary()
    {
        return CreateClient(_options.SecondaryProvider);
    }

    private IHoroscopeAiClient CreateClient(string provider)
    {
        return provider.ToLowerInvariant() switch
        {
            "claude" => new AnthropicClient(
                _httpFactory.CreateClient("Anthropic"),
                _options.AnthropicApiKey,
                _options.AnthropicModel,
                _loggerFactory.CreateLogger<AnthropicClient>()),

            "openai" => new OpenAiClient(
                _httpFactory.CreateClient("OpenAI"),
                _options.OpenAiApiKey,
                _options.OpenAiModel,
                _loggerFactory.CreateLogger<OpenAiClient>()),

            _ => throw new ArgumentException($"Unknown AI provider: {provider}")
        };
    }
}

public class AiOptions
{
    public string PrimaryProvider { get; set; } = "claude";
    public string SecondaryProvider { get; set; } = "openai";
    public string AnthropicApiKey { get; set; } = string.Empty;
    public string AnthropicModel { get; set; } = "claude-sonnet-4-20250514";
    public string OpenAiApiKey { get; set; } = string.Empty;
    public string OpenAiModel { get; set; } = "gpt-4o";
}
