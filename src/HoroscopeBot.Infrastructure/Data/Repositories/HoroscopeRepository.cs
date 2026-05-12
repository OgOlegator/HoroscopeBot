using HoroscopeBot.Domain.Entities;
using HoroscopeBot.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoroscopeBot.Infrastructure.Data.Repositories;

public class HoroscopeRepository : IHoroscopeRepository
{
    private readonly AppDbContext _db;

    public HoroscopeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Horoscope horoscope, CancellationToken ct = default)
    {
        await _db.Horoscopes.AddAsync(horoscope, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Horoscope?> GetTodayByUserAsync(Guid userId, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _db.Horoscopes
            .AsNoTracking()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Date == today, ct);
    }
}
