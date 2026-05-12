namespace HoroscopeBot.Application.DTOs;

public record RegisterUserRequest(
    long TelegramId,
    string? UserName,
    string? FirstName,
    DateOnly BirthDate,
    TimeOnly? BirthTime,
    string? BirthCity,
    string? TimeZone
);
