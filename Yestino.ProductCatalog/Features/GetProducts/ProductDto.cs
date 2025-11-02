namespace Yestino.ProductCatalog.Features.GetProducts;

public record ProductDto
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public string? ImageUrl { get; init; }
    public required decimal Price { get; init; }
}