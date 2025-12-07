using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.Ordering.Domain;
using Yestino.Ordering.Infrastructure;

namespace Yestino.Ordering.Features.ShipOrder;

public static class ShipOrderEndpoint
{
    [WolverinePut("/orders/{orderId}/ship")]
    public static async Task<(IResult, IStorageAction<Order>)> ShipOrder(
        Guid orderId,
        ShipOrderCommand command,
        OrderingDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order == null)
        {
            return (Results.NotFound($"Order {orderId} not found"), 
                   Storage.Nothing<Order>());
        }

        try
        {
            order.Ship(command.TrackingNumber);
            

            return (Results.Ok(new { 
                    OrderId = orderId,
                    Status = order.Status.ToString(),
                    TrackingNumber = order.TrackingNumber,
                    ShippedAt = order.ShippedAt,
                    Message = "Order shipped successfully"
                }), 
                Storage.Update(order));
        }
        catch (InvalidOperationException ex)
        {
            return (Results.BadRequest(ex.Message), 
                   Storage.Nothing<Order>());
        }
        catch (ArgumentException ex)
        {
            return (Results.BadRequest(ex.Message), 
                   Storage.Nothing<Order>());
        }
    }
}