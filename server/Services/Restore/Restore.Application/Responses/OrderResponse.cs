namespace Restore.Application.Responses;

public class OrderResponse
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public AddressResponse ShippingAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemResponse> OrderItems { get; set; }
    public long Subtotal { get; set; }
    public long DeliveryFee { get; set; }
    public OrderStatusResponse OrderStatus { get; set; }
    public long Total { get; set; }
}
