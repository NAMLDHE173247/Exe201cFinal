using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StyleGenie.Infrastructure.Data.Models;
using StyleGenie.Application.Security;
using StyleGenie.Application.Credentials;
using StyleGenie.Application.Wallet;
using StyleGenie.Application.TryOn;
using StyleGenie.Application.Images;
using StyleGenie.Infrastructure.Security;
using StyleGenie.Infrastructure.Persistence;

using StyleGenie.Infrastructure.Services;

namespace StyleGenie.Infrastructure;

public static class GlobalInfrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<TryOnDbContext>(opt =>
            opt.UseSqlServer(cfg.GetConnectionString("Sql")));

        services.AddSingleton<ICrypto, SimpleAesGcmCrypto>();
        services.AddScoped<ICredentialStore, EfCredentialStore>();
        services.AddScoped<IWalletService, EfWalletService>();
        services.AddScoped<IImageReadService, ImageReadService>();

        services.AddHttpClient();
        services.AddScoped<ITryOnProvider, FitRoomProvider>();
        services.AddScoped<ITryOnService, TryOnService>();

        return services;
    }
}
