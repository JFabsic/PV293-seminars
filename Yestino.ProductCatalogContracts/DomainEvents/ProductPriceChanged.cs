using Yestino.Common.Domain;

namespace Yestino.ProductCatalogContracts.DomainEvents;

public record ProductPriceChanged(Guid AggregateId, decimal NewPrice) : DomainEvent(AggregateId);
