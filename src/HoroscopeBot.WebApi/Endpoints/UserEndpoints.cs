using HoroscopeBot.Application.DTOs;
using HoroscopeBot.Application.Interfaces;

namespace HoroscopeBot.WebApi.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users");

        group.MapPost("/register", async (RegisterUserRequest request, IUserService userService, CancellationToken ct) =>
        {
            var user = await userService.RegisterAsync(request, ct);
            return Results.Ok(user);
        });

        group.MapGet("/{telegramId:long}", async (long telegramId, IUserService userService, CancellationToken ct) =>
        {
            var user = await userService.GetByTelegramIdAsync(telegramId, ct);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });
    }
}
