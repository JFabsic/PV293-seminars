using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalog.Features.DeactivateProduct;

namespace Yestino.ProductCatalog.Tests.Features;

public class DeactivateProductTests
{
    [Fact]
    public void DeactivateProduct_WithActiveProduct_ShouldDeactivateAndRaiseEvent()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            IsActive = true
        };

        // Act
        var (result, storageAction, domainEvent) = DeactivateProductEndpoint.DeactivateProduct(product);

        // Assert - Response and storage action
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NoContent>();
        storageAction.Should().NotBeNull();

        // Assert - Entity state
        product.IsActive.Should().BeFalse();

        // Assert - Domain event
        domainEvent.Should().NotBeNull();
        domainEvent!.AggregateId.Should().Be(product.Id);
        domainEvent.OccurredOn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void DeactivateProduct_WithAlreadyInactiveProduct_ShouldReturnBadRequestAndNotRaiseEvent()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            IsActive = false
        };

        // Act
        var (result, storageAction, domainEvent) = DeactivateProductEndpoint.DeactivateProduct(product);

        // Assert - Response
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>>();

        // Assert - Entity state unchanged
        product.IsActive.Should().BeFalse();

        // Assert - No domain event
        domainEvent.Should().BeNull();
    }
}
