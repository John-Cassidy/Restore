namespace Restore.Application.Responses;
public class BasketResponse
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public List<BasketItemResponse> Items { get; set; } = new();
    public string PaymentIntentId { get; set; }
    public string ClientSecret { get; set; }
}