using HoroscopeBot.Application.Interfaces;
using HoroscopeBot.Application.Services;
using HoroscopeBot.Bot.Services;
using HoroscopeBot.Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

var builder = Host.CreateApplicationBuilder(args);

var botToken = builder.Configuration["TelegramBotToken"]
    ?? throw new InvalidOperationException("TelegramBotToken is not configured");

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHoroscopeService, HoroscopeService>();

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddHostedService<HoroscopeSenderService>();

var host = builder.Build();

var bot = host.Services.GetRequiredService<ITelegramBotClient>();
var handler = host.Services.GetRequiredService<UpdateHandler>();

var cts = new CancellationTokenSource();
bot.StartReceiving(
    updateHandler: handler.HandleUpdateAsync,
    errorHandler: handler.HandleErrorAsync,
    receiverOptions: new ReceiverOptions { AllowedUpdates = [] },
    cancellationToken: cts.Token
);

await host.RunAsync();
