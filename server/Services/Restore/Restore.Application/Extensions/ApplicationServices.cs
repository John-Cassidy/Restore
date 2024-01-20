using AutoMapper;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Restore.Application.Handlers;

namespace Restore.Application.Extensions;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}