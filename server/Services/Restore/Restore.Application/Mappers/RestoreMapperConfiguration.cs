using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Restore.Application.Mappers;

// MapperConfiguration is sealed in AutoMapper 13+, so we use composition instead of inheritance.
// AutoMapper 15+ requires an ILoggerFactory; NullLoggerFactory is used as a safe default.
public class RestoreMapperConfiguration
{
    private readonly MapperConfiguration _configuration;

    public RestoreMapperConfiguration(ILoggerFactory? loggerFactory = null)
    {
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.ShouldMapProperty = p => p.GetMethod is { } m && (m.IsPublic || m.IsAssembly);
            cfg.AddProfile<ProductMappingProfile>();
            cfg.AddProfile<BasketMappingProfile>();
            cfg.AddProfile<OrderMappingProfile>();
        }, loggerFactory ?? NullLoggerFactory.Instance);
    }

    /// <summary>Validates all configured mappings. Call only in development/testing.</summary>
    public void AssertConfigurationIsValid() => _configuration.AssertConfigurationIsValid();

    /// <summary>Creates a new <see cref="IMapper"/> from the current configuration.</summary>
    public IMapper CreateMapper() => _configuration.CreateMapper();
}