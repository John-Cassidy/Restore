using AutoMapper;
using Restore.Application.Responses;
using Restore.Core.Entities;
using Restore.Core.Pagination;

namespace Restore.Application.Mappers;

public class ProductMapperConfiguration : MapperConfiguration
{
    public ProductMapperConfiguration() : base(cfg =>
    {
        cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
        cfg.AddProfile<ProductMappingProfile>();
    })
    { }
}