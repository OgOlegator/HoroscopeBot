using HoroscopeBot.Application.DTOs;
using HoroscopeBot.Application.Interfaces;
using HoroscopeBot.Domain.Entities;
using HoroscopeBot.Domain.Interfaces;

namespace HoroscopeBot.Application.Services;

public class HoroscopeService : IHoroscopeService
{
    private readonly IUserRepository _userRepo;
    private readonly IHoroscopeRepository _horoscopeRepo;
    private readonly IHoroscopeGenerator _generator;

    public HoroscopeService(
        IUserRepository userRepo,
        IHoroscopeRepository horoscopeRepo,
        IHoroscopeGenerator generator)
    {
        _userRepo = userRepo;
        _horoscopeRepo = horoscopeRepo;
        _generator = generator;
    }

    public async Task<HoroscopeResponse> GenerateDailyAsync(long telegramId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByTelegramIdAsync(telegramId, ct)
            ?? throw new InvalidOperationException("User not found");

        var existing = await _horoscopeRepo.GetTodayByUserAsync(user.Id, ct);
        if (existing is not null)
            return MapToResponse(existing);

        var text = await _generator.GenerateAsync(user, ct);

        var horoscope = new Horoscope
        {
            UserId = user.Id,
            Date = DateTime.UtcNow.Date,
            Text = text,
            AiProvider = Domain.Enums.AiProvider.Claude,
            GeneratedAt = DateTime.UtcNow
        };

        await _horoscopeRepo.AddAsync(horoscope, ct);
        return MapToResponse(horoscope);
    }

    public async Task GenerateAndSendToAllAsync(Func<long, string, Task> sendMessage, CancellationToken ct = default)
    {
        var users = await _userRepo.GetAllActiveAsync(ct);

        foreach (var user in users)
        {
            try
            {
                var existing = await _horoscopeRepo.GetTodayByUserAsync(user.Id, ct);
                string text;
                Domain.Enums.AiProvider provider;

                if (existing is not null)
                {
                    text = existing.Text;
                    provider = existing.AiProvider;
                }
                else
                {
                    text = await _generator.GenerateAsync(user, ct);
                    provider = Domain.Enums.AiProvider.Claude;

                    var horoscope = new Horoscope
                    {
                        UserId = user.Id,
                        Date = DateTime.UtcNow.Date,
                        Text = text,
                        AiProvider = provider,
                        GeneratedAt = DateTime.UtcNow
                    };

                    await _horoscopeRepo.AddAsync(horoscope, ct);
                }

                var message = FormatHoroscopeMessage(user, text);
                await sendMessage(user.TelegramId, message);
            }
            catch
            {
                // Log and continue
            }
        }
    }

    private static string FormatHoroscopeMessage(Domain.Entities.User user, string text)
    {
        var sign = user.ZodiacSign.ToString();
        var header = $"\U0001f319 \u0413\u043e\u0440\u043e\u0441\u043a\u043e\u043f \u043d\u0430 \u0441\u0435\u0433\u043e\u0434\u043d\u044f, {user.FirstName ?? sign}!\n\n";
        return header + text;
    }

    private static HoroscopeResponse MapToResponse(Horoscope h) =>
        new(h.Id, h.User.ZodiacSign.ToString(), h.Text, h.Date, h.AiProvider.ToString());
}
