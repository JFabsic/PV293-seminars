using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Yestino.Warehouse.Infrastructure;

namespace Yestino.Warehouse.Features.GetProducts;

public static class GetWarehouseProductsEndpoint
{
    [WolverineGet("/warehouse/products")]
    public static async Task<ICollection<WarehouseProductDto>> GetWarehouseProducts(
        [FromQuery] bool onlyActive = false,
        [FromQuery] bool onlyInStock = false,
        WarehouseDbContext dbContext = default!,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.WarehouseProducts.AsQueryable();

        // Apply filters
        if (onlyActive)
        {
            query = query.Where(wp => wp.IsActive);
        }

        if (onlyInStock)
        {
            query = query.Where(wp => wp.StockQuantity > 0);
        }

        return await query
            .Select(wp => new WarehouseProductDto
            {
                Id = wp.Id,
                ProductCatalogId = wp.ProductCatalogId,
                Name = wp.Name,
                Description = wp.Description,
                UnitPrice = wp.UnitPrice,
                StockQuantity = wp.StockQuantity,
                ReservedQuantity = wp.ReservedQuantity,
                AvailableQuantity = wp.StockQuantity - wp.ReservedQuantity,
                IsActive = wp.IsActive,
                CreatedAt = wp.CreatedAt,
                LastStockUpdate = wp.LastStockUpdate
            })
            .OrderBy(wp => wp.Name)
            .ToListAsync(cancellationToken);
    }
}