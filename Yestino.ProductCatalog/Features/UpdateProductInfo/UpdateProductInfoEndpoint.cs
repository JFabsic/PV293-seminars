using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Domain;

namespace Yestino.ProductCatalog.Features.UpdateProductInfo;

public record UpdateProductInfoCommand(string ProductName, string ProductDescription, string? ImageUrl);

public static class UpdateProductInfoEndpoint
{
    [WolverinePut("/product/{productId}")]
    public static IStorageAction<Product> Put([Entity] Product product, UpdateProductInfoCommand command)
    {
        product.Update(command.ProductName, command.ProductDescription, command.ImageUrl);
        return new Update<Product>(product);
    }
}