using HoroscopeBot.Application.Interfaces;

namespace HoroscopeBot.WebApi.Endpoints;

public static class HoroscopeEndpoints
{
    public static void MapHoroscopeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/horoscope");

        group.MapPost("/generate/{telegramId:long}", async (long telegramId, IHoroscopeService horoscopeService, CancellationToken ct) =>
        {
            try
            {
                var result = await horoscopeService.GenerateDailyAsync(telegramId, ct);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
        });
    }
}
