namespace HoroscopeBot.Application.DTOs;

/// <summary>Ответ с данными сгенерированного гороскопа</summary>
public record HoroscopeResponse(
    /// <summary>Уникальный идентификатор записи гороскопа</summary>
    int Id,

    /// <summary>Знак зодиака пользователя (текстом)</summary>
    string ZodiacSign,

    /// <summary>Текст гороскопа, сгенерированный AI</summary>
    string Text,

    /// <summary>Дата, на которую составлен гороскоп</summary>
    DateTime Date,

    /// <summary>Какая AI-модель использовалась (Claude / Gpt)</summary>
    string AiProvider
);
