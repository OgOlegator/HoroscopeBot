using HoroscopeBot.Application.DTOs;
using HoroscopeBot.Application.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HoroscopeBot.Bot.Services;

public class UpdateHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(ITelegramBotClient bot, IServiceScopeFactory scopeFactory, ILogger<UpdateHandler> logger)
    {
        _bot = bot;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        try
        {
            if (update.Message is not { } message)
                return;

            if (message.Text is not { } text)
                return;

            var chatId = message.Chat.Id;

            if (text.StartsWith("/"))
            {
                await HandleCommandAsync(chatId, text, message, ct);
            }
            else if (message.ReplyToMessage?.Text?.Contains("Дата рождения", StringComparison.OrdinalIgnoreCase) == true)
            {
                await HandleBirthDateInput(chatId, text, message, ct);
            }
            else
            {
                await _bot.SendMessage(chatId, "Используй /start для регистрации или /horoscope для получения гороскопа.", cancellationToken: ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling update");
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Polling error");
        return Task.CompletedTask;
    }

    private async Task HandleCommandAsync(long chatId, string text, Message message, CancellationToken ct)
    {
        switch (text.Split(' ')[0])
        {
            case "/start":
                await ShowWelcome(chatId, ct);
                break;

            case "/register":
                await StartRegistration(chatId, ct);
                break;

            case "/horoscope":
                await SendDailyHoroscope(chatId, ct);
                break;

            default:
                await _bot.SendMessage(chatId, "Неизвестная команда. Доступно: /start, /register, /horoscope", cancellationToken: ct);
                break;
        }
    }

    private async Task ShowWelcome(long chatId, CancellationToken ct)
    {
        var msg = "\U0001f31f Привет! Я бот-гороскоп.\n\n"
            + "/register - зарегистрироваться и указать свою дату рождения\n"
            + "/horoscope - получить гороскоп на сегодня";

        await _bot.SendMessage(chatId, msg, cancellationToken: ct);
    }

    private async Task StartRegistration(long chatId, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var exists = await userService.ExistsAsync(chatId, ct);

        if (exists)
        {
            await _bot.SendMessage(chatId,
                "Ты уже зарегистрирован! Используй /horoscope для получения гороскопа.",
                cancellationToken: ct);
            return;
        }

        await _bot.SendMessage(chatId,
            "Давай зарегистрируемся. Напиши свою дату рождения в формате ДД.ММ.ГГГГ\n\n"
            + "Например: 15.03.1990",
            cancellationToken: ct);
    }

    private async Task HandleBirthDateInput(long chatId, string text, Message message, CancellationToken ct)
    {
        if (!DateOnly.TryParseExact(text, ["dd.MM.yyyy", "d.M.yyyy"], null, System.Globalization.DateTimeStyles.None, out var birthDate))
        {
            await _bot.SendMessage(chatId,
                "Неверный формат. Напиши дату как ДД.ММ.ГГГГ, например: 15.03.1990",
                cancellationToken: ct);
            return;
        }

        var request = new RegisterUserRequest(
            TelegramId: chatId,
            UserName: message.From?.Username,
            FirstName: message.From?.FirstName,
            BirthDate: birthDate,
            BirthTime: null,
            BirthCity: null,
            TimeZone: "Europe/Moscow"
        );

        using var scope = _scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var user = await userService.RegisterAsync(request, ct);

        var msg = $"\u2705 Регистрация завершена!\n"
            + $"\ud83d\udcc5 Дата рождения: {birthDate:dd.MM.yyyy}\n"
            + $"\u267b Знак зодиака: {user.ZodiacSign}\n\n"
            + "Теперь ты можешь получать гороскоп каждый день через /horoscope";

        await _bot.SendMessage(chatId, msg, cancellationToken: ct);
    }

    private async Task SendDailyHoroscope(long chatId, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var horoscopeService = scope.ServiceProvider.GetRequiredService<IHoroscopeService>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        var user = await userService.GetByTelegramIdAsync(chatId, ct);
        if (user is null)
        {
            await _bot.SendMessage(chatId,
                "Ты ещё не зарегистрирован. Напиши /register чтобы начать.",
                cancellationToken: ct);
            return;
        }

        try
        {
            var horoscope = await horoscopeService.GenerateDailyAsync(chatId, ct);
            var signEmoji = GetZodiacEmoji(user.ZodiacSign);
            var msg = $"{signEmoji} Гороскоп на сегодня, {user.FirstName ?? user.ZodiacSign.ToString()}!\n\n{horoscope.Text}";
            await _bot.SendMessage(chatId, msg, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate horoscope for {ChatId}", chatId);
            await _bot.SendMessage(chatId, "Не удалось сгенерировать гороскоп. Попробуй позже.", cancellationToken: ct);
        }
    }

    private static string GetZodiacEmoji(Domain.Enums.ZodiacSign sign) => sign switch
    {
        Domain.Enums.ZodiacSign.Aries => "\u2648",
        Domain.Enums.ZodiacSign.Taurus => "\u2649",
        Domain.Enums.ZodiacSign.Gemini => "\u264a",
        Domain.Enums.ZodiacSign.Cancer => "\u264b",
        Domain.Enums.ZodiacSign.Leo => "\u264c",
        Domain.Enums.ZodiacSign.Virgo => "\u264d",
        Domain.Enums.ZodiacSign.Libra => "\u264e",
        Domain.Enums.ZodiacSign.Scorpio => "\u264f",
        Domain.Enums.ZodiacSign.Sagittarius => "\u2650",
        Domain.Enums.ZodiacSign.Capricorn => "\u2651",
        Domain.Enums.ZodiacSign.Aquarius => "\u2652",
        Domain.Enums.ZodiacSign.Pisces => "\u2653",
        _ => "\u2728"
    };
}
