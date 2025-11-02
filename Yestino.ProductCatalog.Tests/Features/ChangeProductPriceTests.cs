using Yestino.ProductCatalog.Entities;
using Yestino.ProductCatalog.Features.ChangeProductPrice;

namespace Yestino.ProductCatalog.Tests.Features;

public class ChangeProductPriceTests
{
    [Fact]
    public void ChangeProductPrice_WithValidPrice_ShouldUpdatePriceAndRaiseEvent()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            IsActive = true
        };

        var command = new ChangeProductPriceCommand(NewPrice: 149.99m);

        // Act
        var (storageAction, domainEvent) = ChangeProductPriceEndpoint.ChangeProductPrice(product, command);

        // Assert - Product price updated
        product.Price.Should().Be(149.99m);

        // Assert - Domain event
        domainEvent.Should().NotBeNull();
        domainEvent.AggregateId.Should().Be(product.Id);
        domainEvent.NewPrice.Should().Be(149.99m);
    }

    [Fact]
    public void ChangeProductPrice_WithZeroPrice_ShouldSucceed()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            IsActive = true
        };

        var command = new ChangeProductPriceCommand(NewPrice: 0m);

        // Act
        var (storageAction, domainEvent) = ChangeProductPriceEndpoint.ChangeProductPrice(product, command);

        // Assert
        product.Price.Should().Be(0m);
        domainEvent.NewPrice.Should().Be(0m);
    }
}
