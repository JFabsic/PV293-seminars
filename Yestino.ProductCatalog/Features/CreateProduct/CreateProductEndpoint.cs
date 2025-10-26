using Microsoft.AspNetCore.Http;
using Wolverine;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Domain;
using Yestino.ProductCatalog.Infrastructure;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.CreateProduct;

public record CreateProductCommand
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
}

public static class CreateProductEndpoint
{
    public static Product? Before(CreateProductCommand command, ProductCatalogDbContext dbContext)
    {
        return dbContext.Products.FirstOrDefault(x => x.Name == command.Name);
    }

    [WolverinePost("/products")]
    public static (IResult, IStorageAction<Product>, ProductCreated?) CreateProduct([NotBody] Product? existingProduct,
        CreateProductCommand command)
    {
        if (existingProduct is not null)
        {
            return (
                Results.BadRequest("Product with the same name already exists"),
                Storage.Nothing<Product>(),
                null
            );
        }

        var product = Product.Create(command.Name, command.Description ?? "", command.ImageUrl);

        return (
            Results.Ok(product),
            Storage.Insert(product),
            new ProductCreated(product.Id, product.Name, product.Description, command.ImageUrl)
        );
    }
}

public static class ProductCreatedHandler
{
    public static void Handle(ProductCreated e, ProductCatalogDbContext dbContext)
    {
        var product = dbContext.Products.FirstOrDefault(x => x.Name == e.Name);
        Console.WriteLine("Product created: {0}", e.Name);
    }
}
