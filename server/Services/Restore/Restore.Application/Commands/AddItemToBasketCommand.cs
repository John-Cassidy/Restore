using MediatR;
using Restore.Application.Responses;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class AddItemToBasketCommand : IRequest<Result<BasketResponse>>
{
    public string BuyerId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public AddItemToBasketCommand(string buyerId, int productId, int quantity)
    {
        BuyerId = buyerId;
        ProductId = productId;
        Quantity = quantity;
    }
}
