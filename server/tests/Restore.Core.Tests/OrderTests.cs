using Restore.Core.Entities.OrderAggregate;

namespace Restore.Core.Tests;

public class OrderTests
{
    [Fact]
    public void GetTotal_ReturnsSubtotalPlusDeliveryFee()
    {
        var order = new Order { Subtotal = 10000, DeliveryFee = 500 };

        Assert.Equal(10500, order.GetTotal());
    }

    [Fact]
    public void GetTotal_WhenFreeDelivery_ReturnsSubtotal()
    {
        var order = new Order { Subtotal = 15000, DeliveryFee = 0 };

        Assert.Equal(15000, order.GetTotal());
    }
}
