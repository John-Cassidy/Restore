namespace Restore.Core.Entities.OrderAggregate;

public class Order
{
    public int Id { get; set; }
    public string BuyerId { get; set; }

    public ShippingAddress ShipToAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public long Subtotal { get; set; }
    public long DeliveryFee { get; set; }
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public long GetTotal()
    {
        return Subtotal + DeliveryFee;
    }
}
