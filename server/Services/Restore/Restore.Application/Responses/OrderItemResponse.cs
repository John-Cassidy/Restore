
namespace Restore.Application.Responses;

public class OrderItemResponse
{
    public int Id { get; set; }
    public ProductItemOrderedResponse ItemOrdered { get; set; }
    public long Price { get; set; }
    public int Quantity { get; set; }
}
