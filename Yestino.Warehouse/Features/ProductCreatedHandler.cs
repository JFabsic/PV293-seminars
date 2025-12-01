using Microsoft.EntityFrameworkCore;
using Wolverine.Persistence;
using Yestino.ProductCatalogContracts.DomainEvents;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features;

public static class ProductCreatedHandler
{
    public static async Task<IStorageAction<WarehouseProduct>?> Handle(
        ProductCreated productCreated, 
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var existingProduct = await dbContext.WarehouseProducts
            .FirstOrDefaultAsync(wp => wp.ProductCatalogId == productCreated.AggregateId, cancellationToken);

        if (existingProduct != null)
        {
            return Storage.Nothing<WarehouseProduct>();
        }

        var warehouseProduct = WarehouseProduct.CreateFromProductCatalog(
            productCreated.AggregateId,
            productCreated.Name,
            productCreated.Description,
            productCreated.Price);

        return Storage.Insert(warehouseProduct);
    }
}