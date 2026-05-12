namespace HoroscopeBot.Application.DTOs;

public record HoroscopeResponse(
    Guid Id,
    string ZodiacSign,
    string Text,
    DateOnly Date,
    string AiProvider
);
