using Yestino.Common.Domain;

namespace Yestino.ProductCatalogContracts.DomainEvents;

public record ProductDeactivated(Guid AggregateId) : DomainEvent(AggregateId);
