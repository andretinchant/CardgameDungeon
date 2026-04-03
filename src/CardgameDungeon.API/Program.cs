using System.Text;
using CardgameDungeon.API.Data.Seeds;
using CardgameDungeon.API.Endpoints;
using CardgameDungeon.API.Hubs;
using CardgameDungeon.API.Infrastructure;
using CardgameDungeon.API.Middleware;
using CardgameDungeon.API.Services;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Behaviors;
using CardgameDungeon.Features.Collection.OpenBooster;
using CardgameDungeon.Features.Match.Shared;
using CardgameDungeon.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// MediatR + FluentValidation
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CardgameDungeon.Features.AssemblyReference>());
builder.Services.AddValidatorsFromAssemblyContaining<CardgameDungeon.Features.AssemblyReference>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Domain services
builder.Services.AddSingleton<CombatResolver>();

// Infrastructure — PostgreSQL + EF Core repositories + Auth services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IBoosterCardPool, RandomBoosterCardPool>();

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var secret = jwtSection["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"] ?? "CardgameDungeon",
        ValidAudience = jwtSection["Audience"] ?? "CardgameDungeon",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ClockSkew = TimeSpan.FromMinutes(1),
        NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub
    };

    // Allow SignalR to receive JWT from query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                context.Token = accessToken;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// SignalR for real-time multiplayer
builder.Services.AddSignalR();
builder.Services.AddScoped<IMatchNotifier, MatchNotificationService>();

// Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CardgameDungeon API",
        Version = "v1",
        Description = "Competitive card game dungeon crawler backend"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seed command: dotnet run --project src/CardgameDungeon.API -- seed
if (args.Contains("seed"))
{
    var sets = CardSetSeeder.CreateAllSets();
    var totalCards = 0;
    foreach (var set in sets)
    {
        Console.WriteLine($"  [{set.Code}] {set.Name} — {set.TotalCards} cards");
        totalCards += set.TotalCards;
    }
    Console.WriteLine($"\nTotal: {sets.Count} sets, {totalCards} cards.");
    return;
}

// Middleware
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "CardgameDungeon v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Health check (public)
app.MapGet("/health", () => Results.Ok("healthy")).WithTags("Health");

// Locale endpoint (public)
app.MapGet("/api/locale", (CardgameDungeon.Features.Localization.ILocalizer localizer) =>
    Results.Ok(new { Locale = localizer.CurrentLocale, Supported = new[] { "en-US", "pt-BR" } }))
    .WithTags("Locale").AllowAnonymous();

// Auth endpoints (public)
app.MapAuthEndpoints();

// SignalR hubs
app.MapHub<MatchHub>("/hubs/match");

// Protected feature endpoints
app.MapDeckEndpoints();
app.MapMatchEndpoints();
app.MapCollectionEndpoints();
app.MapWalletEndpoints();
app.MapMarketplaceEndpoints();
app.MapQueueEndpoints();
app.MapTournamentEndpoints();
app.MapRankingEndpoints();

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program;
