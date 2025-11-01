using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.ActivateProduct;

public static class ActivateProductEndpoint
{
    [WolverinePost("/products/{productId}/activate")]
    public static (IResult, IStorageAction<Product>, ProductActivated?) ActivateProduct([Entity] Product product)
    {
        if (product.IsActive)
        {
            return (
                Results.BadRequest("Product is already active"),
                Storage.Nothing<Product>(),
                null
            );
        }

        return (
            Results.NoContent(),
            Storage.Update(product),
            new ProductActivated(product.Id, product.Name)
        );
    }
}
