using HoroscopeBot.Application.DTOs;
using HoroscopeBot.Application.Interfaces;
using HoroscopeBot.Domain.Entities;
using HoroscopeBot.Domain.Enums;
using HoroscopeBot.Domain.Interfaces;

namespace HoroscopeBot.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken ct = default)
    {
        var existing = await _userRepo.GetByTelegramIdAsync(request.TelegramId, ct);
        if (existing is not null)
        {
            existing.UserName = request.UserName ?? existing.UserName;
            existing.FirstName = request.FirstName ?? existing.FirstName;
            existing.BirthDate = request.BirthDate;
            existing.BirthTime = request.BirthTime;
            existing.BirthCity = request.BirthCity;
            existing.ZodiacSign = CalculateZodiacSign(request.BirthDate);
            existing.TimeZone = request.TimeZone;
            existing.IsActive = true;

            await _userRepo.UpdateAsync(existing, ct);
            return MapToResponse(existing);
        }

        var user = new User
        {
            TelegramId = request.TelegramId,
            UserName = request.UserName,
            FirstName = request.FirstName,
            BirthDate = request.BirthDate,
            BirthTime = request.BirthTime,
            BirthCity = request.BirthCity,
            ZodiacSign = CalculateZodiacSign(request.BirthDate),
            TimeZone = request.TimeZone ?? "Europe/Moscow",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user, ct);
        return MapToResponse(user);
    }

    public async Task<UserResponse?> GetByTelegramIdAsync(long telegramId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByTelegramIdAsync(telegramId, ct);
        return user is null ? null : MapToResponse(user);
    }

    public async Task<bool> ExistsAsync(long telegramId, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByTelegramIdAsync(telegramId, ct);
        return user is not null;
    }

    private static UserResponse MapToResponse(User user) =>
        new(user.Id, user.TelegramId, user.UserName, user.FirstName, user.BirthDate,
            user.BirthTime, user.BirthCity, user.ZodiacSign, user.IsActive);

    internal static ZodiacSign CalculateZodiacSign(DateTime birthDate)
    {
        return (birthDate.Month, birthDate.Day) switch
        {
            (3, >= 21) or (4, <= 19) => ZodiacSign.Aries,
            (4, >= 20) or (5, <= 20) => ZodiacSign.Taurus,
            (5, >= 21) or (6, <= 20) => ZodiacSign.Gemini,
            (6, >= 21) or (7, <= 22) => ZodiacSign.Cancer,
            (7, >= 23) or (8, <= 22) => ZodiacSign.Leo,
            (8, >= 23) or (9, <= 22) => ZodiacSign.Virgo,
            (9, >= 23) or (10, <= 22) => ZodiacSign.Libra,
            (10, >= 23) or (11, <= 21) => ZodiacSign.Scorpio,
            (11, >= 22) or (12, <= 21) => ZodiacSign.Sagittarius,
            (12, >= 22) or (1, <= 19) => ZodiacSign.Capricorn,
            (1, >= 20) or (2, <= 18) => ZodiacSign.Aquarius,
            (2, >= 19) or (3, <= 20) => ZodiacSign.Pisces,
            _ => throw new ArgumentOutOfRangeException(nameof(birthDate), "Invalid date")
        };
    }
}
