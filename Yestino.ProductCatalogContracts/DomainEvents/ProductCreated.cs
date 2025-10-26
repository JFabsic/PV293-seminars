namespace Yestino.ProductCatalogContracts.DomainEvents;

public record ProductCreated(Guid Id, string Name, string Description, string? ImageUrl);