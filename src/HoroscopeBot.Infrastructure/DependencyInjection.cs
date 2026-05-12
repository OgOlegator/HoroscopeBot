using HoroscopeBot.Domain.Interfaces;
using HoroscopeBot.Infrastructure.AI;
using HoroscopeBot.Infrastructure.Data;
using HoroscopeBot.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HoroscopeBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=horoscope_bot;Username=postgres;Password=postgres";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IHoroscopeRepository, HoroscopeRepository>();

        var aiOptions = new AiOptions();
        config.GetSection("AI").Bind(aiOptions);
        services.AddSingleton(aiOptions);

        services.AddHttpClient("Anthropic", client =>
        {
            client.BaseAddress = new Uri("https://api.anthropic.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient("OpenAI", client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddSingleton<AiClientFactory>();
        services.AddScoped<IHoroscopeGenerator, AiHoroscopeGenerator>();

        return services;
    }
}
