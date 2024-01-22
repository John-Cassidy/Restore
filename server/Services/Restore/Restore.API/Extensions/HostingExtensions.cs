using Microsoft.EntityFrameworkCore;
using Restore.API.Endpoints;
using Restore.Application.Handlers;
using Restore.Application.Extensions;
using Restore.Infrastructure.Data;
using Restore.Infrastructure.Extensions;
using Restore.API.Handlers;
using Microsoft.AspNetCore.Http.HttpResults;
using Restore.Core.Exceptions;

namespace Restore.API.Extensions;

public static class HostingExtensions
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        builder.Services.AddLogging();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services.AddCors();

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddExceptionHandler<GeneralExceptionHandler>();
    }

    public static async Task<WebApplication> ConfigurePipeline(this WebApplication app, ConfigurationManager configuration)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (app.Environment.IsProduction())
        {
            app.UseHttpsRedirection();
        }

        app.UseCors(opt =>
        {
            opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(configuration["Cors:ClientAddress"]);
        });

        app.UseExceptionHandler(_ => { });

        app.MapGet("/throwException", (_) => throw new Exception());
        app.MapGet("/throwNotFound", (_) => throw new NotFoundException("Not found"));

        app.AddWeatherForecastEndpoints();
        app.AddProductsEndpoints();

        var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
            context.Database.Migrate();
            context.Database.EnsureCreated();
            await DbInitializer.InitializeAsync(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during migration");
        }

        return app;
    }
}
