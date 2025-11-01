using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.Http;
using Yestino.Common.Infrastructure.Persistence;
using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalogContracts.DomainEvents;

namespace Yestino.ProductCatalog.Features.ActivateProduct;

public class ActivateProductOldSchool
{
    private readonly IAggregateRepository<Product> _productRepository;
    private readonly IMessageBus _bus;

    public ActivateProductOldSchool(IAggregateRepository<Product> productRepository, IMessageBus bus)
    {
        _productRepository = productRepository;
        _bus = bus;
    }

    [WolverinePut("/products/{productId}/activate-old-school")]
    public async Task<IResult> ActivateProduct(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetById(productId, cancellationToken);
        if (product == null)
        {
            return Results.NotFound("Product not found");
        }

        if (product.IsActive)
        {
            return Results.BadRequest("Product is already active");
        }

        product.IsActive = true;

        var domainEvent = new ProductActivated(productId, product.Name);
        await _bus.PublishAsync(domainEvent);

        return Results.NoContent();
    }
}