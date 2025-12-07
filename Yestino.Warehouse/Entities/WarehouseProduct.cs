using Yestino.Common.Domain;
using Yestino.WarehouseContracts.DomainEvents;

namespace Yestino.Warehouse.Entities;

public class WarehouseProduct : AggregateRoot
{
    public Guid ProductCatalogId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int StockQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastStockUpdate { get; private set; }

    // EF Core constructor
    private WarehouseProduct()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public static WarehouseProduct CreateFromProductCatalog(
        Guid productCatalogId, 
        string name, 
        string description, 
        decimal price)
    {
        var warehouseProduct = new WarehouseProduct
        {
            ProductCatalogId = productCatalogId,
            Name = name,
            Description = description,
            UnitPrice = price,
            StockQuantity = 0, // Initially no stock
            ReservedQuantity = 0,
            IsActive = false, // Initially inactive until stock is added
            CreatedAt = DateTime.UtcNow
        };

        return warehouseProduct;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        var previousStockQuantity = StockQuantity;
        var wasActive = IsActive;
        
        StockQuantity += quantity;
        LastStockUpdate = DateTime.UtcNow;
        
        if (!IsActive && StockQuantity > 0)
        {
            IsActive = true;
        }

        RaiseDomainEvent(new WarehouseStockUpdated(
            Id,
            ProductCatalogId,
            StockQuantity,
            quantity,
            IsActive,
            DateTime.UtcNow
        ));
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (AvailableQuantity < quantity)
            throw new InvalidOperationException("Insufficient stock available");

        ReservedQuantity += quantity;
        LastStockUpdate = DateTime.UtcNow;
    }

    public void ReleaseReservedStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException("Cannot release more stock than reserved");

        ReservedQuantity -= quantity;
        LastStockUpdate = DateTime.UtcNow;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (StockQuantity < quantity)
            throw new InvalidOperationException("Cannot remove more stock than available");

        var previousStockQuantity = StockQuantity;
        
        StockQuantity -= quantity;
        LastStockUpdate = DateTime.UtcNow;

        if (StockQuantity == 0)
        {
            IsActive = false;
        }

        this.RaiseDomainEvent(new WarehouseStockUpdated(
            Id,
            ProductCatalogId,
            StockQuantity,
            -quantity,
            IsActive,
            DateTime.UtcNow
        ));
    }

    public int AvailableQuantity => StockQuantity - ReservedQuantity;

    public void UpdateProductInfo(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        UnitPrice = price;
    }
}