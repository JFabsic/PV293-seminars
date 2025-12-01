using Microsoft.EntityFrameworkCore;
using Wolverine.Persistence;
using Yestino.Ordering.Application;
using Yestino.Ordering.Infrastructure;
using Yestino.WarehouseContracts.DomainEvents;

namespace Yestino.Ordering.Features;

public static class WarehouseStockUpdatedHandler
{
    public static async Task<IStorageAction<ProductReadModel>?> Handle(
        WarehouseStockUpdated stockUpdated,
        OrderingDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var productReadModel = await dbContext.ProductReadModels
            .FirstOrDefaultAsync(p => p.Id == stockUpdated.ProductCatalogId, cancellationToken);

        if (productReadModel == null)
        {
            // Product read model doesn't exist in ordering context
            return Storage.Nothing<ProductReadModel>();
        }

        // Update the existing tracked entity's stock quantity
        productReadModel.StockQuantity = stockUpdated.NewStockQuantity;

        return Storage.Update(productReadModel);
    }
}