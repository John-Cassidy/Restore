namespace Restore.Application.Responses;

public class BasketItemResponse
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public ProductResponse Product { get; set; } = new();
}