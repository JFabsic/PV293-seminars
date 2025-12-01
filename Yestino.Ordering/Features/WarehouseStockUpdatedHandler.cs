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
            return Storage.Nothing<ProductReadModel>();
        }

        var updatedReadModel = productReadModel with 
        { 
            StockQuantity = stockUpdated.NewStockQuantity 
        };

        return Storage.Update(updatedReadModel);
    }
}