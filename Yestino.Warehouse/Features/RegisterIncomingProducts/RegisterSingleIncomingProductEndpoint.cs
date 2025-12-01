using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features.RegisterIncomingProducts;

public record RegisterSingleIncomingProductCommand(int Quantity);

public static class RegisterSingleIncomingProductEndpoint
{
    [WolverinePut("/warehouse/products/{productCatalogId}/incoming")]
    public static async Task<(IResult, IStorageAction<WarehouseProduct>)> RegisterIncomingProduct(
        Guid productCatalogId,
        RegisterSingleIncomingProductCommand command,
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (command.Quantity <= 0)
        {
            return (Results.BadRequest("Quantity must be positive"), 
                   Storage.Nothing<WarehouseProduct>());
        }

        var warehouseProduct = await dbContext.WarehouseProducts
            .FirstOrDefaultAsync(wp => wp.ProductCatalogId == productCatalogId, cancellationToken);

        if (warehouseProduct == null)
        {
            return (Results.NotFound($"Warehouse product with ProductCatalogId {productCatalogId} not found"), 
                    Storage.Nothing<WarehouseProduct>());
        }

        try
        {
            warehouseProduct.AddStock(command.Quantity);
            
            // Domain events are automatically published by DbContextBase.SaveChangesAsync()
            // No need to return them explicitly
            
            return (Results.Ok(new { 
                    ProductCatalogId = productCatalogId,
                    AddedQuantity = command.Quantity,
                    NewStockQuantity = warehouseProduct.StockQuantity,
                    Message = "Stock successfully added"
                }), 
                Storage.Update(warehouseProduct));
        }
        catch (Exception ex)
        {
            return (Results.BadRequest($"Error adding stock: {ex.Message}"), 
                   Storage.Nothing<WarehouseProduct>());
        }
    }
}