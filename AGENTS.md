# HoroscopeBot — правила для AI-ассистента

## Стек
.NET 9, C#, EF Core + PostgreSQL, Telegram.Bot 22.x, Minimal API, BackgroundService

## Структура
Multi-project: Domain → Application → Infrastructure → WebApi + Bot

## Конвенции
- Все DTO — `record`
- Всегда передавать `CancellationToken ct` последним параметром
- `DateOnly` для дат, `TimeOnly` для времени, `long` для TelegramId
- Комментарии НЕ добавлять
- ZodiacSign — `enum` в коде, `string` в БД (`HasConversion<string>`)
- Новые AI провайдеры: реализовать `IHoroscopeAiClient`, добавить в `AiClientFactory`
- Новые эндпоинты: extension methods на `WebApplication` в `Endpoints/`
- Новые кроны: `BackgroundService`, без Quartz/Hangfire
- Один гороскоп на пользователя в день — всегда проверять `GetTodayByUserAsync`
- Ошибки AI логировать через `LogWarning`, fallback на второй провайдер
- Миграции: `dotnet ef migrations add Name --project src/HoroscopeBot.Infrastructure --startup-project src/HoroscopeBot.WebApi`
- Для быстрой проверки сборки: `dotnet build`

## Запуск
```powershell
docker compose up -d                        # PostgreSQL
$env:TelegramBotToken = "..."
$env:AI__AnthropicApiKey = "..."
# затем:
dotnet run --project src\HoroscopeBot.WebApi
dotnet run --project src\HoroscopeBot.Bot
```
