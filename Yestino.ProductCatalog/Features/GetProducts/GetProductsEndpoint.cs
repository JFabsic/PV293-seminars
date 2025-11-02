using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine.Http;
using Yestino.ProductCatalog.Infrastructure;

namespace Yestino.ProductCatalog.Features.GetProducts;

public static class GetProductsEndpoint
{
    [WolverineGet("/products")]
    public static async Task<ICollection<ProductDto>> GetProducts([FromQuery] bool onlyActive,
        ProductCatalogDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .Where(x => !onlyActive || x.IsActive)
            .Select(x => new ProductDto
            {
                Name = x.Name,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                Price = x.Price,
            })
            .ToListAsync(cancellationToken);
    }
}