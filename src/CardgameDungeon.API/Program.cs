using CardgameDungeon.API.Data.Seeds;
using CardgameDungeon.API.Endpoints;
using CardgameDungeon.API.Infrastructure;
using CardgameDungeon.API.Middleware;
using CardgameDungeon.Domain.Services;
using CardgameDungeon.Features.Behaviors;
using CardgameDungeon.Features.Collection.OpenBooster;
using CardgameDungeon.Infrastructure;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// MediatR + FluentValidation
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CardgameDungeon.Features.AssemblyReference>());
builder.Services.AddValidatorsFromAssemblyContaining<CardgameDungeon.Features.AssemblyReference>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Domain services
builder.Services.AddSingleton<CombatResolver>();

// Infrastructure — PostgreSQL + EF Core repositories
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IBoosterCardPool, RandomBoosterCardPool>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CardgameDungeon API",
        Version = "v1",
        Description = "Competitive card game dungeon crawler backend"
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

// Health check
app.MapGet("/health", () => Results.Ok("healthy")).WithTags("Health");

// Feature endpoints
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
