using Microsoft.AspNetCore.Http;
using Wolverine;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.DeactivateProduct;

public static class DeactivateProductEndpoint
{
    [WolverinePost("/products/{productId}/deactivate")]
    public static (IResult, IStorageAction<Product>, ProductDeactivated?) DeactivateProduct([Entity] Product product, IMessageBus bus)
    {
        if (product.IsActive)
        {
            return (
                Results.BadRequest("Product with the same name already exists"),
                Storage.Nothing<Product>(),
                null);
        }

        return (
            Results.Ok(product),
            Storage.Update(product),
            new ProductDeactivated(product.Id, product.Name)
        );
    }
}
