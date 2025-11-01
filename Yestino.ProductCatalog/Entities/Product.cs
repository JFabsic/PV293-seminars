using Yestino.Common.Domain;

namespace Yestino.ProductCatalog.Entities;

public class Product : AggregateRoot
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}