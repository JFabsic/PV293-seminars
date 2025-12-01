using Yestino.Ordering.Application;
using Yestino.Ordering.Infrastructure;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.Ordering.Features.EventHandlers;

public static class CreateProductReadModelOnProductCreatedHandler
{
    public static void Handle(ProductCreated domainEvent, OrderingDbContext dbContext)
    {
        var productModel = new ProductReadModel
        {
            Name = domainEvent.Name,
            Price = domainEvent.Price,
            Description = domainEvent.Description,
            ImageUrl =  domainEvent.ImageUrl,
            StockQuantity = 0,
            Id = domainEvent.AggregateId,
        };
        
        dbContext.ProductReadModels.Add(productModel);
    }
}