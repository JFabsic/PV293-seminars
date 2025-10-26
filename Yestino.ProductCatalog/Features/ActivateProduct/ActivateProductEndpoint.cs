using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Domain;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.ActivateProduct;

public static class ActivateProductEndpoint
{
    [WolverinePost("/products/{productId}/activate")]
    public static (IResult, IStorageAction<Product>, ProductActivated?) ActivateProduct([Entity] Product product)
    {
        product.Activate();

        return (
            Results.Ok(product),
            Storage.Update(product),
            new ProductActivated(product.Id, product.Name)
        );
    }
}
