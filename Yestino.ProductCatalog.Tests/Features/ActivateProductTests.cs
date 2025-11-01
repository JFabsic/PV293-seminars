using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalog.Features.ActivateProduct;

namespace Yestino.ProductCatalog.Tests.Features;

public class ActivateProductTests
{
    [Fact]
    public void ActivateProduct_WithInactiveProduct_ShouldActivateAndRaiseEvent()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            IsActive = false
        };

        // Act
        var (result, storageAction, domainEvent) = ActivateProductEndpoint.ActivateProduct(product);

        // Assert - Response and storage action
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.NoContent>();
        storageAction.Should().NotBeNull();

        // Assert - Entity state
        product.IsActive.Should().BeTrue();

        // Assert - Domain event
        domainEvent.Should().NotBeNull();
        domainEvent!.AggregateId.Should().Be(product.Id);
        domainEvent.Name.Should().Be(product.Name);
        domainEvent.OccurredOn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ActivateProduct_WithAlreadyActiveProduct_ShouldReturnBadRequestAndNotRaiseEvent()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            IsActive = true
        };

        // Act
        var (result, storageAction, domainEvent) = ActivateProductEndpoint.ActivateProduct(product);

        // Assert - Response
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<string>>();

        // Assert - Entity state unchanged
        product.IsActive.Should().BeTrue();

        // Assert - No domain event
        domainEvent.Should().BeNull();
    }
}
