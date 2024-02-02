using AutoMapper;
using MediatR;
using Restore.Application.Commands;
using Restore.Application.Responses;
using Restore.Core.Entities;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateOrderHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var basket = await _unitOfWork.BasketRepository.ReadAsync(command.BuyerId);

        if (basket == null || !basket.Items.Any())
        {
            return Result<OrderResponse>.Failure("Could not locate basket");
        }

        var items = new List<OrderItem>();

        foreach (var item in basket.Items)
        {
            var productItem = await _unitOfWork.ProductRepository.ReadAsync(item.ProductId);
            if (productItem == null)
            {
                return Result<OrderResponse>.Failure("Product not found");
            }
            var itemOrdered = new ProductItemOrdered
            {
                ProductId = productItem.Id,
                Name = productItem.Name,
                PictureUrl = productItem.PictureUrl
            };
            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quantity = item.Quantity
            };
            items.Add(orderItem);
            productItem.QuantityInStock -= item.Quantity;
        }

        var subtotal = items.Sum(item => item.Price * item.Quantity);
        var deliveryFee = subtotal > 10000 ? 0 : 500;

        var ShippingAddress = _mapper.Map<ShippingAddress>(command.ShippingAddress);

        var order = new Order
        {
            OrderItems = items,
            BuyerId = command.BuyerId,
            ShippingAddress = ShippingAddress,
            Subtotal = subtotal,
            DeliveryFee = deliveryFee
        };

        await _unitOfWork.OrderRepository.AddAsync(order);
        await _unitOfWork.BasketRepository.DeleteAsync(basket);

        if (command.SaveAddress)
        {
            var user = await _unitOfWork.UserRepository.ReadUserAddressAsync(command.UserName);
            if (user == null)
            {
                return Result<OrderResponse>.Failure("User not found");
            }
            var address = new UserAddress
            {
                Id = user.Address?.Id ?? 0,
                FullName = command.ShippingAddress.FullName,
                Address1 = command.ShippingAddress.Address1,
                Address2 = command.ShippingAddress.Address2,
                City = command.ShippingAddress.City,
                State = command.ShippingAddress.State,
                Zip = command.ShippingAddress.Zip,
                Country = command.ShippingAddress.Country
            };
            user.Address = address;

            await _unitOfWork.UserRepository.UpdateAsync(user);
        }

        var result = await _unitOfWork.CompleteAsync() > 0;

        if (!result)
        {
            return Result<OrderResponse>.Failure("Problem saving changes");
        }

        return Result<OrderResponse>.Success(_mapper.Map<OrderResponse>(order));
    }
}
