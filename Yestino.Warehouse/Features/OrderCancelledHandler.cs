using Microsoft.EntityFrameworkCore;
using Wolverine.Persistence;
using Yestino.OrderingContracts.DomainEvents;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features;

public static class OrderCancelledHandler
{
    public static async Task<IStorageAction<WarehouseProduct>[]> Handle(
        OrderCancelled orderCancelled,
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var productIds = orderCancelled.Items.Select(i => i.ProductId).ToList();

        var warehouseProducts = await dbContext.WarehouseProducts
            .Where(wp => productIds.Contains(wp.ProductCatalogId))
            .ToDictionaryAsync(wp => wp.ProductCatalogId, cancellationToken);

        var storageActions = new List<IStorageAction<WarehouseProduct>>();

        foreach (var orderItem in orderCancelled.Items)
        {
            if (warehouseProducts.TryGetValue(orderItem.ProductId, out var warehouseProduct))
            {
                try
                {
                    warehouseProduct.ReleaseReservedStock(orderItem.Quantity);
                    storageActions.Add(Storage.Update(warehouseProduct));
                }
                catch (InvalidOperationException ex)
                {
                    continue;
                }
            }
        }

        return storageActions.ToArray();
    }
}