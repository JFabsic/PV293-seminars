using Yestino.Common.Domain;

namespace Yestino.OrderingContracts.DomainEvents;

public record OrderShipped(
    Guid AggregateId, 
    ICollection<OrderShippedItem> Items,
    string TrackingNumber,
    DateTimeOffset ShippedAt
) : DomainEvent(AggregateId);

public record OrderShippedItem(Guid ProductId, string ProductName, int Quantity, decimal Price);