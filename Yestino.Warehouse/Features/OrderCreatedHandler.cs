using Microsoft.EntityFrameworkCore;
using Yestino.OrderingContracts.DomainEvents;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features;

public static class OrderCreatedHandler
{
    public static async Task Handle(
        OrderCreated orderCreated,
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var productIds = orderCreated.Items.Select(i => i.ProductId).ToList();
        
        var warehouseProducts = await dbContext.WarehouseProducts
            .Where(wp => productIds.Contains(wp.ProductCatalogId))
            .ToDictionaryAsync(wp => wp.ProductCatalogId, cancellationToken);

        foreach (var orderItem in orderCreated.Items)
        {
            if (warehouseProducts.TryGetValue(orderItem.ProductId, out var warehouseProduct))
            {
                try
                {
                    warehouseProduct.ReserveStock(orderItem.Quantity);
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