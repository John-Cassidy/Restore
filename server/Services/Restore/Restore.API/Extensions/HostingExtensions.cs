using Microsoft.EntityFrameworkCore;
using Restore.API.Endpoints;
using Restore.Application.Handlers;
using Restore.Application.Extensions;
using Restore.Infrastructure.Data;
using Restore.Infrastructure.Extensions;
using Restore.API.Handlers;
using Microsoft.AspNetCore.Http.HttpResults;
using Restore.Core.Exceptions;
using System.Runtime.Serialization;
using Restore.Core;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Restore.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Restore.API.Extensions;

public static class HostingExtensions
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IValidationExceptionHandler, ValidationExceptionHandler>();

        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services.AddCors();

        builder.Services.AddIdentityServices(builder.Configuration);

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

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionEndpoints();

        app.AddWeatherForecastEndpoints();
        app.AddProductsEndpoints();
        app.AddBasketEndpoints();
        app.AddAccountEndpoints();

        var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        try
        {

            await context.Database.MigrateAsync();
            await context.Database.EnsureCreatedAsync();
            await DbInitializer.InitializeAsync(context, userManager);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during migration");
        }

        return app;
    }
}