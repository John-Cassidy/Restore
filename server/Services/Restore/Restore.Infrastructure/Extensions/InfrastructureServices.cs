using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restore.Core.Repositories;
using Restore.Infrastructure.Data;
using Restore.Infrastructure.Repositories;

namespace Restore.Infrastructure.Extensions;

public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (connectionString == null)
        {
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        services.AddDbContext<StoreContext>(options =>
            options.UseSqlite(connectionString), ServiceLifetime.Scoped);

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();

        return services;
    }
}
