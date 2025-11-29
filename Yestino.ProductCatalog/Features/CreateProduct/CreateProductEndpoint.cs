using Microsoft.AspNetCore.Http;
using Wolverine.Http;
using Wolverine.Persistence;
using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalog.Infrastructure;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.CreateProduct;

public record CreateProductCommand
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public required decimal Price { get; init; }
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

        if (command.Price < 0)
        {
            return (
                Results.BadRequest("Price cannot be negative"),
                Storage.Nothing<Product>(),
                null
            );
        }

        var product = new Product
        {
            Name = command.Name,
            Description = command.Description ?? "",
            ImageUrl = command.ImageUrl,
            Price = command.Price,
        };

        return (
            Results.Ok(product.Id),
            Storage.Insert(product),
            new ProductCreated(product.Id, product.Name, product.Description, command.ImageUrl, command.Price)
        );
    }
}