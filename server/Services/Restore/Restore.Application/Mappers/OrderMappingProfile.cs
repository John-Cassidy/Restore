using AutoMapper;
using Restore.Application.Requests;
using Restore.Application.Responses;
using Restore.Core.Entities.OrderAggregate;

namespace Restore.Application.Mappers;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderResponse>().ReverseMap();
        CreateMap<ShippingAddress, AddressResponse>().ReverseMap();
        CreateMap<ShippingAddress, AddressRequest>().ReverseMap();
        CreateMap<OrderItem, OrderItemResponse>().ReverseMap();
        CreateMap<ProductItemOrdered, ProductItemOrderedResponse>().ReverseMap();
        CreateMap<OrderStatus, OrderStatusResponse>().ReverseMap();
    }
}
