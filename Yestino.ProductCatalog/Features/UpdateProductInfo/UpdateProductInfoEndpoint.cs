using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.UpdateProductInfo;

public record UpdateProductInfoCommand(string ProductName, string ProductDescription, string? ImageUrl);

public static class UpdateProductInfoEndpoint
{
    [WolverinePut("/products/{productId}")]
    public static (IStorageAction<Product>, ProductInfoUpdated) UpdateProductInfo([Entity] Product product, UpdateProductInfoCommand command)
    {
        product.Name = command.ProductName;
        product.Description = command.ProductDescription;
        product.ImageUrl = command.ImageUrl;
        
        var domainEvent = new ProductInfoUpdated(product.Id, command.ProductName, command.ProductDescription, command.ImageUrl);

        return (Storage.Update(product), domainEvent);
    }
}