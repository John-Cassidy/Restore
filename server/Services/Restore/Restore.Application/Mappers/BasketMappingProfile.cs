using AutoMapper;
using Restore.Application.Responses;
using Restore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restore.Application.Mappers;
public partial class BasketMappingProfile : Profile {
    public BasketMappingProfile() {
        CreateMap<Basket, BasketResponse>().ReverseMap();
        CreateMap<BasketItem, BasketItemResponse>().ReverseMap();
    }
}
