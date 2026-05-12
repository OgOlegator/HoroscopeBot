using HoroscopeBot.Domain.Enums;

namespace HoroscopeBot.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public long TelegramId { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public DateOnly BirthDate { get; set; }
    public TimeOnly? BirthTime { get; set; }
    public string? BirthCity { get; set; }
    public ZodiacSign ZodiacSign { get; set; }
    public string? TimeZone { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Horoscope> Horoscopes { get; set; } = new List<Horoscope>();
}
