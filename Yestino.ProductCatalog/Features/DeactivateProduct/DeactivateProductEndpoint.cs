using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.DeactivateProduct;

public static class DeactivateProductEndpoint
{
    [WolverinePost("/products/{productId}/deactivate")]
    public static (IResult, IStorageAction<Product>, ProductDeactivated?) DeactivateProduct([Entity] Product product)
    {
        if (!product.IsActive)
        {
            return (
                Results.BadRequest("Product is already deactivated"),
                Storage.Nothing<Product>(),
                null);
        }

        product.IsActive = false;

        return (
            Results.NoContent(),
            Storage.Update(product),
            new ProductDeactivated(product.Id, product.Name)
        );
    }
}