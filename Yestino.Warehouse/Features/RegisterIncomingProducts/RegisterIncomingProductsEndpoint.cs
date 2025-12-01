using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.Warehouse.Entities;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features.RegisterIncomingProducts;

public static class RegisterIncomingProductsEndpoint
{
    [WolverinePost("/warehouse/incoming-products")]
    public static async Task<IResult> RegisterIncomingProducts(
        RegisterIncomingProductsCommand command,
        WarehouseDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        if (command.Items == null || !command.Items.Any())
        {
            return Results.BadRequest("No products specified");
        }

        var productCatalogIds = command.Items.Select(i => i.ProductCatalogId).ToList();
        
        var existingWarehouseProducts = await dbContext.WarehouseProducts
            .Where(wp => productCatalogIds.Contains(wp.ProductCatalogId))
            .ToDictionaryAsync(wp => wp.ProductCatalogId, cancellationToken);

        var errors = new List<string>();
        var updatedProducts = new List<WarehouseProduct>();

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
                updatedProducts.Add(warehouseProduct);
            }
            catch (Exception ex)
            {
                errors.Add($"Error adding stock for product {item.ProductCatalogId}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            return Results.BadRequest(new { Errors = errors });
        }

        // Save changes directly - domain events will be published automatically
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(new { Message = $"Successfully registered {command.Items.Count} incoming products" });
    }
}