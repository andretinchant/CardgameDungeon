using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Auth;
using CardgameDungeon.Features.Localization;
using CardgameDungeon.Infrastructure.Auth;
using CardgameDungeon.Infrastructure.Data;
using CardgameDungeon.Infrastructure.Localization;
using CardgameDungeon.Infrastructure.Redis;
using CardgameDungeon.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CardgameDungeon.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CardgameDungeonDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<ICardRepository, EfCardRepository>();
        services.AddScoped<IDeckRepository, EfDeckRepository>();
        services.AddScoped<IMatchRepository, EfMatchRepository>();
        services.AddScoped<ICollectionRepository, EfCollectionRepository>();
        services.AddScoped<IWalletRepository, EfWalletRepository>();
        services.AddScoped<IMarketplaceRepository, EfMarketplaceRepository>();
        services.AddScoped<IQueueRepository, EfQueueRepository>();
        services.AddScoped<IRatingRepository, EfRatingRepository>();
        services.AddScoped<ITournamentRepository, EfTournamentRepository>();
        services.AddScoped<IPlayerRepository, EfPlayerRepository>();
        services.AddScoped<IRefreshTokenRepository, EfRefreshTokenRepository>();

        // Auth services
        services.AddSingleton<IAuthTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        // Localization
        services.AddHttpContextAccessor();
        services.AddScoped<ILocalizer, RequestLocalizer>();

        // Redis (optional — falls back to EF if not configured)
        var redisConnectionString = configuration["Redis:ConnectionString"];
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(redisConnectionString));

            // Replace EF queue repo with Redis-backed one
            services.AddScoped<IQueueRepository, RedisMatchmakingQueue>();
            services.AddSingleton<RedisSessionCache>();
        }

        return services;
    }
}
