using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.Ordering.Domain;
using Yestino.Ordering.Infrastructure;

namespace Yestino.Ordering.Features.CancelOrder;

public static class CancelOrderEndpoint
{
    [WolverinePut("/orders/{orderId}/cancel")]
    public static async Task<(IResult, IStorageAction<Order>)> CancelOrder(
        Guid orderId,
        CancelOrderCommand command,
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
            order.Cancel();

            return (Results.Ok(new { 
                    OrderId = orderId,
                    Status = order.Status.ToString(),
                    CancellationReason = command.Reason,
                    Message = "Order cancelled successfully"
                }), 
                Storage.Update(order));
        }
        catch (InvalidOperationException ex)
        {
            return (Results.BadRequest(ex.Message), 
                   Storage.Nothing<Order>());
        }
    }
}