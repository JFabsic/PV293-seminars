namespace Yestino.Warehouse.Features.GetProducts;

public record WarehouseProductDto
{
    public Guid Id { get; init; }
    public Guid ProductCatalogId { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal UnitPrice { get; init; }
    public int StockQuantity { get; init; }
    public int ReservedQuantity { get; init; }
    public int AvailableQuantity { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastStockUpdate { get; init; }
}