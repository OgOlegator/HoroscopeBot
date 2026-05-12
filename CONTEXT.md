# HoroscopeBot — Telegram бот-гороскоп с AI

## Архитектура

Multi-project solution (.NET 9, C#), PostgreSQL + EF Core, Telegram.Bot, AI (Anthropic Claude + OpenAI GPT).

```
Последовательность вызовов при ежедневной рассылке (9:00 MSK):

HoroscopeSenderService (крон)
  → HoroscopeService.GenerateAndSendToAllAsync(callback)
    → UserRepository.GetAllActiveAsync()
    → для каждого пользователя:
        1. HoroscopeRepository.GetTodayByUserAsync() — проверка дубля
        2. AiHoroscopeGenerator.GenerateAsync(user)
            → AiClientFactory.CreatePrimary() → AnthropicClient
            → при ошибке: CreateSecondary() → OpenAiClient
        3. HoroscopeRepository.AddAsync(horoscope)
        4. callback(telegramId, message) → Telegram.Bot API
```

## Структура проекта

```
HoroscopeBot/
├── docker-compose.yml          # PostgreSQL 16-alpine
├── .env                        # шаблон переменных (токены, ключи AI)
├── CONTEXT.md                  # этот файл
├── HoroscopeBot.sln
└── src/
    ├── HoroscopeBot.Domain/          [classlib]
    │   ├── Entities/
    │   │   ├── User.cs              TelegramId, BirthDate, BirthTime, BirthCity, ZodiacSign, TimeZone, IsActive
    │   │   └── Horoscope.cs         UserId, Date, Text, AiProvider
    │   ├── Enums/
    │   │   ├── ZodiacSign.cs        12 знаков (Aries..Pisces)
    │   │   └── AiProvider.cs        Claude, Gpt
    │   └── Interfaces/
    │       ├── IUserRepository.cs       GetByTelegramId, GetAllActive, Add, Update
    │       ├── IHoroscopeRepository.cs  Add, GetTodayByUser
    │       └── IHoroscopeGenerator.cs   GenerateAsync(User)
    │
    ├── HoroscopeBot.Application/  [classlib]
    │   ├── DTOs/
    │   │   ├── RegisterUserRequest.cs   TelegramId, UserName, FirstName, BirthDate, BirthTime, BirthCity, TimeZone
    │   │   ├── UserResponse.cs          Id, TelegramId, UserName, FirstName, BirthDate.., ZodiacSign, IsActive
    │   │   └── HoroscopeResponse.cs     Id, ZodiacSign, Text, Date, AiProvider
    │   ├── Interfaces/
    │   │   ├── IUserService.cs          Register, GetByTelegramId, Exists
    │   │   └── IHoroscopeService.cs     GenerateDaily(tgId), GenerateAndSendToAll(callback)
    │   └── Services/
    │       ├── UserService.cs           регистрация/обновление, CalculateZodiacSign
    │       └── HoroscopeService.cs      генерация с проверкой дубля, массовая рассылка
    │
    ├── HoroscopeBot.Infrastructure/ [classlib]
    │   ├── Data/
    │   │   ├── AppDbContext.cs
    │   │   ├── Configurations/
    │   │   │   ├── UserConfiguration.cs        TelegramId — unique index
    │   │   │   └── HoroscopeConfiguration.cs   UserId+Date — composite index
    │   │   └── Repositories/
    │   │       ├── UserRepository.cs
    │   │       └── HoroscopeRepository.cs
    │   ├── AI/
    │   │   ├── IHoroscopeAiClient.cs          GenerateHoroscopeAsync(prompt)
    │   │   ├── AnthropicClient.cs             POST /v1/messages (Anthropic API)
    │   │   ├── OpenAiClient.cs                POST /v1/chat/completions (OpenAI API)
    │   │   ├── AiClientFactory.cs             создаёт клиента по имени провайдера
    │   │   ├── AiHoroscopeGenerator.cs        IHoroscopeGenerator — build prompt, primary→fallback
    │   │   └── AiOptions.cs                   Primary/SecondaryProvider, ApiKey, Model
    │   └── DependencyInjection.cs             AddInfrastructure(config)
    │
    ├── HoroscopeBot.WebApi/         [webapi]
    │   ├── Endpoints/
    │   │   ├── UserEndpoints.cs         POST /api/users/register, GET /api/users/{telegramId}
    │   │   └── HoroscopeEndpoints.cs    POST /api/horoscope/generate/{telegramId}
    │   └── Program.cs
    │
    └── HoroscopeBot.Bot/            [worker]
        ├── Services/
        │   ├── UpdateHandler.cs          обработка команд /start, /register, /horoscope
        │   └── HoroscopeSenderService.cs BackgroundService — крон в 6:00 UTC (9:00 MSK)
        └── Program.cs
```

## Ключевые решения

| Решение | Почему |
|---------|--------|
| Multi-project (Domain/Application/Infrastructure/WebApi/Bot) | Чистая архитектура, разделение ответственности |
| Telegram.Bot 22.x | Стандарт для .NET Telegram ботов |
| Пулл-модель (StartReceiving) | Проще для старта, не требует webhook/SSL |
| AI fallback: Claude → OpenAI | Отказоустойчивость: при ошибке Claude — GPT |
| Один гороскоп на пользователя в день | Проверка в `GetTodayByUserAsync`, не дублируем |
| BackgroundService для крона | Встроенный механизм .NET, не нужен внешний планировщик |
| Minimal API | Минимум бойлерплейта для простых эндпоинтов |
| ZodiacSign как string в БД | Читаемость миграций, enum хранится как текст |

## Настройка

```env
TelegramBotToken=your_token
AI__PrimaryProvider=claude
AI__SecondaryProvider=openai
AI__AnthropicApiKey=sk-ant-...
AI__AnthropicModel=claude-sonnet-4-20250514
AI__OpenAiApiKey=sk-...
AI__OpenAiModel=gpt-4o
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=horoscope_bot;Username=postgres;Password=postgres
```

## DI-контейнер

- `Infrastructure/DependencyInjection.cs` — регистрирует DbContext, репозитории, HttpClient для Anthropic/OpenAI, AiClientFactory, IHoroscopeGenerator
- WebApi/Bot Program.cs добавляют IUserService и IHoroscopeService (из Application)
- Bot дополнительно регистрирует ITelegramBotClient, UpdateHandler, HoroscopeSenderService

## Потоки

### Регистрация пользователя
```
/register → UpdateHandler → запрос даты рождения
→ пользователь пишет "15.03.1990"
→ UserService.RegisterAsync → CalculateZodiacSign → UserRepository.AddAsync
→ ответ со знаком зодиака
```

### Получение гороскопа
```
/horoscope → UpdateHandler → HoroscopeService.GenerateDailyAsync(telegramId)
→ проверка существования пользователя
→ HoroscopeRepository.GetTodayByUserAsync (есть ли уже?)
→ если нет: AiHoroscopeGenerator.GenerateAsync → AnthropicClient / OpenAiClient
→ HoroscopeRepository.AddAsync
→ ответ пользователю
```

### Ежедневная рассылка (6:00 UTC / 9:00 MSK)
```
HoroscopeSenderService.ExecuteAsync → Task.Delay до 6:00 UTC
→ HoroscopeService.GenerateAndSendToAllAsync(sendMessage)
→ для каждого активного пользователя:
  1. проверить/создать гороскоп
  2. TelegramBotClient.SendMessageAsync
```

## Рекомендации для AI-разработки

1. **Не добавляй комментарии** — код самодокументируемый, используй expressive naming
2. **DTO через record** — все DTO уже record'ы, новые делай так же
3. **AI клиенты** — если новый провайдер: реализуй `IHoroscopeAiClient`, добавь в `AiClientFactory`, укажи провайдера в AiOptions
4. **BackgroundService** — для новых кронов используй `BackgroundService`, не Quartz/Hangfire
5. **Minimal API** — новые эндпоинты в `Endpoints/` как extension methods на `WebApplication`
6. **EF Core миграции** — `dotnet ef migrations add` из корня
7. **ZodiacSign** — строка в БД (`HasConversion<string>`), enum в коде — не менять формат
8. **Типы** — `DateOnly` для дат, `TimeOnly` для времени, `long` для TelegramId
9. **Проверка дублей** — гороскоп на день только один, всегда проверяй `GetTodayByUserAsync`
10. **Ошибки AI** — логировать через `ILogger.LogWarning`, пробрасывать исключения только если оба провайдера упали
11. **CancellationToken** — всегда передавать `ct` во все async методы
