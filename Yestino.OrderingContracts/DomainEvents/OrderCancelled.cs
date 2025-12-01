using Yestino.Common.Domain;

namespace Yestino.OrderingContracts.DomainEvents;

public record OrderCancelled(Guid AggregateId, ICollection<OrderCancelledItem> Items) : DomainEvent(AggregateId);

public record OrderCancelledItem(Guid ProductId, string ProductName, int Quantity, decimal Price);