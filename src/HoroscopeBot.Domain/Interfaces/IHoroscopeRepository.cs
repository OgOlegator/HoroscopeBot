using HoroscopeBot.Domain.Entities;

namespace HoroscopeBot.Domain.Interfaces;

public interface IHoroscopeRepository
{
    Task AddAsync(Horoscope horoscope, CancellationToken ct = default);
    Task<Horoscope?> GetTodayByUserAsync(int userId, CancellationToken ct = default);
}
