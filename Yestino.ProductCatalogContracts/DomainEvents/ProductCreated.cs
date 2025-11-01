using Yestino.Common.Domain;

namespace Yestino.ProductCatalogContracts.DomainEvents;

public record ProductCreated(Guid AggregateId, string Name, string Description, string? ImageUrl) : DomainEvent(AggregateId);