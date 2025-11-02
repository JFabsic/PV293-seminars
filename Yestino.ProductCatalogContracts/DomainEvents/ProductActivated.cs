using Yestino.Common.Domain;

namespace Yestino.ProductCatalogContracts.DomainEvents;

public record ProductActivated(Guid AggregateId) : DomainEvent(AggregateId);
