using HoroscopeBot.Domain.Enums;

namespace HoroscopeBot.Application.DTOs;

public record UserResponse(
    Guid Id,
    long TelegramId,
    string? UserName,
    string? FirstName,
    DateOnly BirthDate,
    TimeOnly? BirthTime,
    string? BirthCity,
    ZodiacSign ZodiacSign,
    bool IsActive
);
