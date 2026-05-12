using HoroscopeBot.Domain.Enums;

namespace HoroscopeBot.Domain.Entities;

public class Horoscope
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateOnly Date { get; set; }
    public string Text { get; set; } = string.Empty;
    public AiProvider AiProvider { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
