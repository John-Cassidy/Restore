﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restore.Application.Abstractions.Authentication;
using Restore.Application.Services;
using Restore.Core.Repositories;
using Restore.Infrastructure.Authentication;
using Restore.Infrastructure.Data;
using Restore.Infrastructure.Repositories;
using Restore.Infrastructure.Services;

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
            options.UseNpgsql(connectionString), ServiceLifetime.Scoped);

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
