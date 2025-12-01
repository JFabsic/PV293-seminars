using Microsoft.EntityFrameworkCore;
using Wolverine.Persistence;
using Yestino.OrderingContracts.DomainEvents;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features;

public static class OrderShippedHandler
{
    public static async Task<IStorageAction<WarehouseProduct>[]> Handle(
        OrderShipped orderShipped,
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var productIds = orderShipped.Items.Select(i => i.ProductId).ToList();
        
        var warehouseProducts = await dbContext.WarehouseProducts
            .Where(wp => productIds.Contains(wp.ProductCatalogId))
            .ToDictionaryAsync(wp => wp.ProductCatalogId, cancellationToken);

        var storageActions = new List<IStorageAction<WarehouseProduct>>();

        foreach (var orderItem in orderShipped.Items)
        {
            if (warehouseProducts.TryGetValue(orderItem.ProductId, out var warehouseProduct))
            {
                try
                {
                    warehouseProduct.RemoveStock(orderItem.Quantity);
                    storageActions.Add(Storage.Update(warehouseProduct));
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
            }
        }

        return storageActions.ToArray();
    }
}