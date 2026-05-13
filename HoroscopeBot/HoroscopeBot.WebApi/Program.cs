using HoroscopeBot.Application.Interfaces;
using HoroscopeBot.Application.Services;
using HoroscopeBot.Infrastructure;
using HoroscopeBot.WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHoroscopeService, HoroscopeService>();

var app = builder.Build();

app.MapUserEndpoints();
app.MapHoroscopeEndpoints();

app.Run();
