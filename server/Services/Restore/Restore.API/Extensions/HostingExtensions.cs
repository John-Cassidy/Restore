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

    public static WebApplication ConfigurePipeline(this WebApplication app)
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

        return app;
    }
}
