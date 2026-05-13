using HoroscopeBot.Domain.Entities;
using HoroscopeBot.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HoroscopeBot.Infrastructure.AI;

public class AiHoroscopeGenerator : IHoroscopeGenerator
{
    private readonly AiClientFactory _clientFactory;
    private readonly ILogger<AiHoroscopeGenerator> _logger;

    public AiHoroscopeGenerator(AiClientFactory clientFactory, ILogger<AiHoroscopeGenerator> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<string> GenerateAsync(User user, CancellationToken ct = default)
    {
        var prompt = BuildPrompt(user);

        try
        {
            var primary = _clientFactory.CreatePrimary();
            return await primary.GenerateHoroscopeAsync(prompt, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Primary AI provider failed, falling back to secondary");

            var secondary = _clientFactory.CreateSecondary();
            return await secondary.GenerateHoroscopeAsync(prompt, ct);
        }
    }

    private static string BuildPrompt(User user)
    {
        var timeInfo = user.BirthTime.HasValue
            ? $", время рождения: {user.BirthTime:HH:mm}"
            : "";
        var cityInfo = !string.IsNullOrEmpty(user.BirthCity)
            ? $", город рождения: {user.BirthCity}"
            : "";

        return $"""
Ты — астролог. Составь подробный ежедневный гороскоп для пользователя.

Знак зодиака: {user.ZodiacSign}
Дата рождения: {user.BirthDate:dd.MM}{timeInfo}{cityInfo}

Напиши гороскоп на сегодня в позитивном ключе. 
Раздели на сферы: карьера, любовь, здоровье.
Используй понятный язык, 1-2 абзаца.
""";
    }
}
