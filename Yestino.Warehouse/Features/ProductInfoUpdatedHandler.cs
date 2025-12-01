using Microsoft.EntityFrameworkCore;
using Wolverine.Persistence;
using Yestino.ProductCatalogContracts.DomainEvents;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features;

public static class ProductInfoUpdatedHandler
{
    public static async Task<IStorageAction<WarehouseProduct>?> Handle(
        ProductInfoUpdated productInfoUpdated,
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var warehouseProduct = await dbContext.WarehouseProducts
            .FirstOrDefaultAsync(wp => wp.ProductCatalogId == productInfoUpdated.AggregateId, cancellationToken);

        if (warehouseProduct == null)
        {
            return Storage.Nothing<WarehouseProduct>();
        }

        warehouseProduct.UpdateProductInfo(
            productInfoUpdated.Name,
            productInfoUpdated.Description ?? string.Empty,
            0);

        return Storage.Update(warehouseProduct);
    }
}