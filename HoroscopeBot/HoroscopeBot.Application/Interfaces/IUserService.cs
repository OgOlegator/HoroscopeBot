using HoroscopeBot.Application.DTOs;

namespace HoroscopeBot.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken ct = default);
    Task<UserResponse?> GetByTelegramIdAsync(long telegramId, CancellationToken ct = default);
    Task<bool> ExistsAsync(long telegramId, CancellationToken ct = default);
}
