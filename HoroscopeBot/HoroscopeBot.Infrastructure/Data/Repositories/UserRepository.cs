using HoroscopeBot.Domain.Entities;
using HoroscopeBot.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoroscopeBot.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken ct = default)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TelegramId == telegramId, ct);
    }

    public async Task<List<User>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _db.Users.AddAsync(user, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }
}
