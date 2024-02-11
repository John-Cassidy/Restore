using AutoMapper;
using Restore.Application.Responses;
using Restore.Core.Entities;

namespace Restore.Application.Mappers;
public partial class BasketMappingProfile : Profile {
    public BasketMappingProfile() {
        CreateMap<Basket, BasketResponse>().ReverseMap();
        CreateMap<BasketItem, BasketItemResponse>().ReverseMap();
    }
}
