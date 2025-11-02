using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.ChangeProductPrice;

public record ChangeProductPriceCommand(decimal NewPrice);

public static class ChangeProductPriceEndpoint
{
    [WolverinePut("/products/{productId}/price")]
    public static (IResult, IStorageAction<Product>, ProductPriceChanged?) ChangeProductPrice([Entity] Product product, ChangeProductPriceCommand command)
    {
        if (command.NewPrice < 0)
        {
            return (
                Results.BadRequest("Price cannot be negative"),
                Storage.Nothing<Product>(),
                null
            );
        }

        product.Price = command.NewPrice;

        var domainEvent = new ProductPriceChanged(product.Id, command.NewPrice);

        return (Results.NoContent(), Storage.Update(product), domainEvent);
    }
}
