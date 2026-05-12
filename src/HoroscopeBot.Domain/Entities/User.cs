using HoroscopeBot.Domain.Enums;

namespace HoroscopeBot.Domain.Entities;

/// <summary>Пользователь телеграм-бота, зарегистрированный для получения гороскопов</summary>
public class User
{
    /// <summary>Уникальный идентификатор пользователя в системе</summary>
    public int Id { get; set; }

    /// <summary>Идентификатор пользователя в Telegram (ChatId), используется для отправки сообщений</summary>
    public long TelegramId { get; set; }

    /// <summary>Имя пользователя в Telegram (@username)</summary>
    public string? UserName { get; set; }

    /// <summary>Имя, указанное в Telegram (First Name), используется в приветствии</summary>
    public string? FirstName { get; set; }

    /// <summary>Дата рождения пользователя, по ней определяется знак зодиака (хранится с временем 00:00:00)</summary>
    public DateTime BirthDate { get; set; }

    /// <summary>Время рождения (опционально), для более точного астрологического прогноза</summary>
    public TimeOnly? BirthTime { get; set; }

    /// <summary>Город рождения (опционально), для географической привязки в гороскопе</summary>
    public string? BirthCity { get; set; }

    /// <summary>Знак зодиака, рассчитанный автоматически из даты рождения</summary>
    public ZodiacSign ZodiacSign { get; set; }

    /// <summary>Часовой пояс пользователя (например Europe/Moscow), для отправки гороскопа в правильное время</summary>
    public string? TimeZone { get; set; }

    /// <summary>Флаг активности — неактивным пользователям гороскоп не отправляется</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Дата и время регистрации пользователя в системе</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Коллекция полученных гороскопов пользователя</summary>
    public ICollection<Horoscope> Horoscopes { get; set; } = new List<Horoscope>();
}
