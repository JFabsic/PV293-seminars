using Yestino.Common.Domain;

namespace Yestino.OrderingContracts.DomainEvents;

public record OrderCreated(Guid AggregateId, ICollection<OrderCreatedItem> Items) : DomainEvent(AggregateId);

public record OrderCreatedItem(Guid ProductId, string ProductName, int Quantity, decimal Price);