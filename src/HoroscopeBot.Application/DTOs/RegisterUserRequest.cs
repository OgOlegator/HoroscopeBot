namespace HoroscopeBot.Application.DTOs;

/// <summary>Запрос на регистрацию нового пользователя</summary>
public record RegisterUserRequest(
    /// <summary>Идентификатор пользователя в Telegram (ChatId)</summary>
    long TelegramId,

    /// <summary>Имя пользователя в Telegram (@username)</summary>
    string? UserName,

    /// <summary>Имя, указанное в Telegram</summary>
    string? FirstName,

    /// <summary>Дата рождения (хранится с временем 00:00:00)</summary>
    DateTime BirthDate,

    /// <summary>Время рождения (опционально)</summary>
    TimeOnly? BirthTime,

    /// <summary>Город рождения (опционально)</summary>
    string? BirthCity,

    /// <summary>Часовой пояс пользователя (например Europe/Moscow)</summary>
    string? TimeZone
);
