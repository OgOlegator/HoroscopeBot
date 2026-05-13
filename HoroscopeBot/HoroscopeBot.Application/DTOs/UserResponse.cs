using HoroscopeBot.Domain.Enums;

namespace HoroscopeBot.Application.DTOs;

/// <summary>
/// Ответ с данными зарегистрированного пользователя
/// </summary>
public record UserResponse(
    /// <summary>
    /// Уникальный идентификатор пользователя в системе
    /// </summary>
    int Id,

    /// <summary>
    /// Идентификатор пользователя в Telegram, используется для отправки сообщений
    /// </summary>
    long TelegramId,

    /// <summary>
    /// Имя пользователя в Telegram (@username)
    /// </summary>
    string? UserName,

    /// <summary>
    /// Имя, указанное в Telegram
    /// </summary>
    string? FirstName,

    /// <summary>
    /// Дата рождения пользователя
    /// (хранится с временем 00:00:00)
    /// </summary>
    DateTime BirthDate,

    /// <summary>
    /// Время рождения (опционально)
    /// </summary>
    TimeOnly? BirthTime,

    /// <summary>
    /// Город рождения (опционально)
    /// </summary>
    string? BirthCity,

    /// <summary>
    /// Знак зодиака, рассчитанный из даты рождения
    /// </summary>
    ZodiacSign ZodiacSign,

    /// <summary>
    /// Флаг активности пользователя
    /// </summary>
    bool IsActive
);
