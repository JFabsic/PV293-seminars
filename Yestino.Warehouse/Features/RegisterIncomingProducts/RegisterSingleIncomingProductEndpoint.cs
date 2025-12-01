using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;
using Yestino.WarehouseContracts.DomainEvents;

namespace Yestino.Warehouse.Features.RegisterIncomingProducts;

public record RegisterSingleIncomingProductCommand(int Quantity);

public static class RegisterSingleIncomingProductEndpoint
{
    [WolverinePut("/warehouse/products/{productCatalogId}/incoming")]
    public static async Task<(IResult, IStorageAction<WarehouseProduct>, WarehouseStockUpdated?)> RegisterIncomingProduct(
        Guid productCatalogId,
        RegisterSingleIncomingProductCommand command,
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (command.Quantity <= 0)
        {
            return (Results.BadRequest("Quantity must be positive"), 
                   Storage.Nothing<WarehouseProduct>(),
                   null);
        }

        var warehouseProduct = await dbContext.WarehouseProducts
            .FirstOrDefaultAsync(wp => wp.ProductCatalogId == productCatalogId, cancellationToken);

        if (warehouseProduct == null)
        {
            return (Results.NotFound($"Warehouse product with ProductCatalogId {productCatalogId} not found"), 
                    Storage.Nothing<WarehouseProduct>(),
                    null);
        }

        try
        {
            warehouseProduct.AddStock(command.Quantity);
            
            var domainEvent = warehouseProduct.DomainEvents
                .OfType<WarehouseStockUpdated>()
                .FirstOrDefault();
            
            return (Results.Ok(new { 
                    ProductCatalogId = productCatalogId,
                    AddedQuantity = command.Quantity,
                    NewStockQuantity = warehouseProduct.StockQuantity,
                    Message = "Stock successfully added"
                }), 
                Storage.Update(warehouseProduct),
                domainEvent);
        }
        catch (Exception ex)
        {
            return (Results.BadRequest($"Error adding stock: {ex.Message}"), 
                   Storage.Nothing<WarehouseProduct>(),
                   null);
        }
    }
}