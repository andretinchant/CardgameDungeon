using CardgameDungeon.Domain.Repositories;
using CardgameDungeon.Features.Auth;
using CardgameDungeon.Infrastructure.Auth;
using CardgameDungeon.Infrastructure.Data;
using CardgameDungeon.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}
