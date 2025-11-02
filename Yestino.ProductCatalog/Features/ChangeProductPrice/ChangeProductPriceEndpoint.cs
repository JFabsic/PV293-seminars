using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.ChangeProductPrice;

public record ChangeProductPriceCommand(decimal NewPrice);

public static class ChangeProductPriceEndpoint
{
    [WolverinePut("/products/{productId}/price")]
    public static (IStorageAction<Product>, ProductPriceChanged) ChangeProductPrice([Entity] Product product, ChangeProductPriceCommand command)
    {
        product.Price = command.NewPrice;

        var domainEvent = new ProductPriceChanged(product.Id, command.NewPrice);

        return (Storage.Update(product), domainEvent);
    }
}
