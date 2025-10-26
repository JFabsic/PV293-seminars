using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Domain;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.DeactivateProduct;

public static class DeactivateProductEndpoint
{
    [WolverinePost("/products/{productId}/deactivate")]
    public static (IResult, IStorageAction<Product>, ProductDeactivated?) DeactivateProduct([Entity] Product product)
    {
        product.Deactivate();

        return (
            Results.Ok(product),
            Storage.Update(product),
            new ProductDeactivated(product.Id, product.Name)
        );
    }
}
