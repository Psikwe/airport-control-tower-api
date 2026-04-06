using AirportControlTower.Api.MIddleware;
using AirportControlTower.Application.Abstractions;
using AirportControlTower.Application.Services;
using AirportControlTower.Application.Services.Interfaces;
using AirportControlTower.Domain.Settings;
using AirportControlTower.Infrastructure.BackgroundServices;
using AirportControlTower.Infrastructure.Data;
using AirportControlTower.Infrastructure.Repositories;
using AirportControlTower.Shared.Configs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IAircraftService, AircraftService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAircraftRepository, EfAircraftRepository>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("basic", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "basic",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Basic Authentication (username:password)"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddScoped<IGroundCrewService, GroundCrewService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

builder.Services.AddHostedService<GroundCrewWorker>();

builder.Services.Configure<AirportSettings>(
    builder.Configuration.GetSection("AirportSettings"));
builder.Services.Configure<AdminCredentials>(
    builder.Configuration.GetSection("AdminCredentials"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await db.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseMiddleware<AdminAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
