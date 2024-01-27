using AutoMapper;
using Restore.Application.Responses;
using Restore.Core.Entities;
using Restore.Core.Pagination;

namespace Restore.Application.Mappers;

public partial class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponse>().ReverseMap();
        CreateMap<PagedList<Product>, PagedList<ProductResponse>>().ReverseMap();
    }
}