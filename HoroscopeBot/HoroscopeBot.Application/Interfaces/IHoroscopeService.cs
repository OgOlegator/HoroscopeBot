using HoroscopeBot.Application.DTOs;

namespace HoroscopeBot.Application.Interfaces;

public interface IHoroscopeService
{
    Task<HoroscopeResponse> GenerateDailyAsync(long telegramId, CancellationToken ct = default);
    Task GenerateAndSendToAllAsync(Func<long, string, Task> sendMessage, CancellationToken ct = default);
}
