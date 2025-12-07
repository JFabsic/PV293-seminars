using Microsoft.EntityFrameworkCore;
using Yestino.OrderingContracts.DomainEvents;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features;

public static class OrderShippedHandler
{
    public static async Task Handle(
        OrderShipped orderShipped,
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var productIds = orderShipped.Items.Select(i => i.ProductId).ToList();
        
        var warehouseProducts = await dbContext.WarehouseProducts
            .Where(wp => productIds.Contains(wp.ProductCatalogId))
            .ToDictionaryAsync(wp => wp.ProductCatalogId, cancellationToken);

        foreach (var orderItem in orderShipped.Items)
        {
            if (warehouseProducts.TryGetValue(orderItem.ProductId, out var warehouseProduct))
            {
                try
                {
                    warehouseProduct.RemoveStock(orderItem.Quantity);
                    warehouseProduct.ReleaseReservedStock(orderItem.Quantity);
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
            }
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}