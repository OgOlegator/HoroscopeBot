using HoroscopeBot.Domain.Entities;

namespace HoroscopeBot.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken ct = default);
    Task<List<User>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
}
