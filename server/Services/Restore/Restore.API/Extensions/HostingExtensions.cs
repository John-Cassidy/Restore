using Restore.API.Endpoints;
using Carter;
using Restore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Restore.Infrastructure.Extensions;

namespace Restore.API.Extensions;

public static class HostingExtensions
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCarter();

        builder.Services.AddInfrastructureServices(builder.Configuration);
    }

    public static async Task<WebApplication> ConfigurePipeline(this WebApplication app)
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

        app.AddWeatherForecastEndpoints();
        app.MapCarter(); // app.MapFallbackToController("Index", "Fallback");

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
