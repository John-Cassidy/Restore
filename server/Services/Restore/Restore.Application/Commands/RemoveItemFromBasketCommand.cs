using MediatR;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class RemoveItemFromBasketCommand : IRequest<Result<bool>>
{
    public string BuyerId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public RemoveItemFromBasketCommand(string buyerId, int productId, int quantity)
    {
        BuyerId = buyerId;
        ProductId = productId;
        Quantity = quantity;
    }
}
