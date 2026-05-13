using HoroscopeBot.Domain.Entities;

namespace HoroscopeBot.Domain.Interfaces;

public interface IHoroscopeGenerator
{
    Task<string> GenerateAsync(User user, CancellationToken ct = default);
}
