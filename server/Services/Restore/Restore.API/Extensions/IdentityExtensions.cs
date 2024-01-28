using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Restore.Core.Entities;
using Restore.Infrastructure.Data;

namespace Restore.API.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<User>(opt =>
        {
            opt.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<StoreContext>();

        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        // .AddJwtBearer();

        // services.ConfigureOptions<JwtOptionsConfiguration>();
        // services.ConfigureOptions<JwtBearerOptionsConfiguration>();
        // // .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
        // // {
        // //     ValidateIssuer = true,
        // //     ValidateAudience = ""
        // // });
        // // .AddJwtBearer(options =>
        // // {
        // //     options.Authority = "https://localhost:5001";
        // //     options.Audience = "api";
        // // });

        services.AddAuthentication();
        services.AddAuthorization();

        return services;
    }
}
