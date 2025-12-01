using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;
using Yestino.WarehouseContracts.DomainEvents;

namespace Yestino.Warehouse.Features.RegisterIncomingProducts;

public static class RegisterIncomingProductsEndpoint
{
    [WolverinePost("/warehouse/incoming-products")]
    public static async Task<(IResult, IStorageAction<WarehouseProduct>[], WarehouseStockUpdated[])> RegisterIncomingProducts(
        RegisterIncomingProductsCommand command,
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (command.Items == null || !command.Items.Any())
        {
            return (Results.BadRequest("No products specified"), 
                   Array.Empty<IStorageAction<WarehouseProduct>>(),
                   Array.Empty<WarehouseStockUpdated>());
        }

        var productCatalogIds = command.Items.Select(i => i.ProductCatalogId).ToList();
        
        var existingWarehouseProducts = await dbContext.WarehouseProducts
            .Where(wp => productCatalogIds.Contains(wp.ProductCatalogId))
            .ToDictionaryAsync(wp => wp.ProductCatalogId, cancellationToken);

        var storageActions = new List<IStorageAction<WarehouseProduct>>();
        var domainEvents = new List<WarehouseStockUpdated>();
        var errors = new List<string>();

        foreach (var item in command.Items)
        {
            if (item.Quantity <= 0)
            {
                errors.Add($"Invalid quantity {item.Quantity} for product {item.ProductCatalogId}. Quantity must be positive.");
                continue;
            }

            if (!existingWarehouseProducts.TryGetValue(item.ProductCatalogId, out var warehouseProduct))
            {
                errors.Add($"Warehouse product with ProductCatalogId {item.ProductCatalogId} not found.");
                continue;
            }

            try
            {
                warehouseProduct.AddStock(item.Quantity);
                storageActions.Add(Storage.Update(warehouseProduct));
                
                domainEvents.AddRange(warehouseProduct.DomainEvents.OfType<WarehouseStockUpdated>());
            }
            catch (Exception ex)
            {
                errors.Add($"Error adding stock for product {item.ProductCatalogId}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return (Results.BadRequest(new { Errors = errors }), 
                   Array.Empty<IStorageAction<WarehouseProduct>>(),
                   Array.Empty<WarehouseStockUpdated>());
        }

        return (Results.Ok(new { Message = $"Successfully registered {command.Items.Count} incoming products" }), 
                storageActions.ToArray(),
                domainEvents.ToArray());
    }
}