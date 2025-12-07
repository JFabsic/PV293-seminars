using Yestino.Common.Domain;

namespace Yestino.WarehouseContracts.DomainEvents;

public record WarehouseStockUpdated(
    Guid AggregateId,
    Guid ProductCatalogId,
    int NewStockQuantity,
    int StockChange,
    bool IsActive,
    DateTime UpdatedAt
) : DomainEvent(AggregateId);