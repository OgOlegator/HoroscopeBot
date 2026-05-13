using HoroscopeBot.Domain.Enums;

namespace HoroscopeBot.Domain.Entities;

/// <summary>Гороскоп, сгенерированный AI для пользователя на определённую дату</summary>
public class Horoscope
{
    /// <summary>Уникальный идентификатор записи гороскопа</summary>
    public int Id { get; set; }

    /// <summary>Идентификатор пользователя, для которого создан гороскоп</summary>
    public int UserId { get; set; }

    /// <summary>Навигационное свойство — пользователь-получатель гороскопа</summary>
    public User User { get; set; } = null!;

    /// <summary>Дата, на которую составлен гороскоп (хранится с временем 00:00:00)</summary>
    public DateTime Date { get; set; }

    /// <summary>Текст гороскопа, сгенерированный AI-моделью</summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>Какая AI-модель использовалась для генерации (Claude или GPT)</summary>
    public AiProvider AiProvider { get; set; }

    /// <summary>Дата и время генерации гороскопа</summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
