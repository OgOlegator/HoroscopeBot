using HoroscopeBot.Application.Interfaces;
using Telegram.Bot;

namespace HoroscopeBot.Bot.Services;

public class HoroscopeSenderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ITelegramBotClient _bot;
    private readonly ILogger<HoroscopeSenderService> _logger;

    public HoroscopeSenderService(IServiceScopeFactory scopeFactory, ITelegramBotClient bot, ILogger<HoroscopeSenderService> logger)
    {
        _scopeFactory = scopeFactory;
        _bot = bot;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var target = now.Date.AddHours(6); // 9 AM MSK (UTC+3)

            if (now > target)
                target = target.AddDays(1);

            var delay = target - now;
            _logger.LogInformation("Next horoscope delivery scheduled at {Target} (in {Delay})", target, delay);

            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;

            await SendHoroscopesToAll(stoppingToken);
        }
    }

    private async Task SendHoroscopesToAll(CancellationToken ct)
    {
        _logger.LogInformation("Starting daily horoscope delivery");

        using var scope = _scopeFactory.CreateScope();
        var horoscopeService = scope.ServiceProvider.GetRequiredService<IHoroscopeService>();

        await horoscopeService.GenerateAndSendToAllAsync(async (telegramId, message) =>
        {
            try
            {
                await _bot.SendMessage(telegramId, message, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send horoscope to {TelegramId}", telegramId);
            }
        }, ct);

        _logger.LogInformation("Daily horoscope delivery completed");
    }
}
