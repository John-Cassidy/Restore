using AutoMapper;

namespace Restore.Application.Mappers;

public class RestoreMapperConfiguration : MapperConfiguration
{
    public RestoreMapperConfiguration() : base(cfg =>
    {
        cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
        cfg.AddProfile<ProductMappingProfile>();
        cfg.AddProfile<BasketMappingProfile>();
        cfg.AddProfile<OrderMappingProfile>();
    })
    { }
}