using Yestino.Common.Domain;

namespace Yestino.ProductCatalog.Domain;

public class Product : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? ImageUrl { get; private set; }

    public static Product Create(string name, string description, string? imageUrl)
    {
        return new Product
        {
            Name = name,
            Description = description,
            ImageUrl = imageUrl,
        };
    }

    public void Update(string name, string description, string? imageUrl)
    {
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
    }
}