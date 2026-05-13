namespace HoroscopeBot.Infrastructure.AI;

public interface IHoroscopeAiClient
{
    Task<string> GenerateHoroscopeAsync(string prompt, CancellationToken ct = default);
}
